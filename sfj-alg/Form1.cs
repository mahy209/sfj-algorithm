using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sfj_alg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int num_process;
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            num_process = (int)numericUpDown1.Value;

            // reset and start again
            button1.Enabled = true;
            // gant.Clear();
            flowLayoutPanel2.Controls.Clear();
            flowLayoutPanel1.Controls.Clear();
            sjwtime = 0;
            sjaverageWtime = 0;
            prsjaverageWtime = 0;
            prsjwtime = 0;
            pd = 1;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label3.Visible = true;
                numericUpDown3.Visible = true;
            }
            else
            {
                label3.Visible = false;
                numericUpDown3.Visible = false;
            }
        }

        // store values accending
        double sjwtime = 0, sjaverageWtime = 0, prsjaverageWtime = 0, prsjwtime = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            Process x = new Process(); // dummy process
            x.Pid = pd;
            pd++;
            x.burst_time = (int)numericUpDown2.Value;
            if (checkBox1.Checked) x.arrival_time = (int)numericUpDown3.Value;
            vals.Add(x);
            string ss_sjf = "Inserted " + (vals.Count.ToString()) + " from " + (num_process.ToString());
            MessageBox.Show(ss_sjf);

            if (vals.Count == num_process && !checkBox1.Checked)
            {
                button1.Enabled = false;
                vals = vals.OrderBy(arr => arr.burst_time).ToList();
                int st_t = 0;
                for (int j = 0; j < num_process; j++)
                {
                    //gantt chart code
                    ///////// type time lable
                    Label num_fcfs = new Label();
                    num_fcfs.Location = new Point(10 + st_t * 10, 305);
                    num_fcfs.Text = st_t.ToString();
                    if (vals[j].burst_time < 3) num_fcfs.Width = 28;
                    else num_fcfs.Width = vals[j].burst_time * 10 - 2;
                    flowLayoutPanel2.Controls.Add(num_fcfs);
                    /////////
                    st_t += vals[j].burst_time;
                    //// type process number lable
                    Label tempLabel_fcfs = new Label();
                    tempLabel_fcfs.Enabled = true;
                    tempLabel_fcfs.BorderStyle = BorderStyle.FixedSingle;
                    tempLabel_fcfs.Font = new Font("Arial", 10, FontStyle.Bold);
                    tempLabel_fcfs.Location = new Point(10 + time_fcfs * 10, 300);
                    string s = "P" + vals[j].Pid.ToString();
                    tempLabel_fcfs.Text = s;
                    tempLabel_fcfs.TextAlign = ContentAlignment.MiddleCenter;
                    if (vals[j].burst_time < 3) tempLabel_fcfs.Width = 30;
                    else tempLabel_fcfs.Width = vals[j].burst_time * 10;
                    flowLayoutPanel1.Controls.Add(tempLabel_fcfs);
                    ////////////////////////

                    //cal average waiting time
                    sjaverageWtime += sjwtime;
                    sjwtime += vals[j].burst_time;
                }
                //print last timelable in gant
                Label num_fcfss = new Label();
                num_fcfss.Location = new Point(10 + st_t * 10, 300);
                num_fcfss.Text = st_t.ToString();
                flowLayoutPanel2.Controls.Add(num_fcfss);
                /////////////////

                //print average time
                textBox1.Text = (sjaverageWtime / (double)num_process).ToString() + " msec";
                vals.Clear();
            }

            //preemtive code
            else if (vals.Count == num_process && checkBox1.Checked)
            {
                button1.Enabled = false;
                vals = vals.OrderBy(arr => arr.arrival_time).ToList();
                int indx = 0;
                for (int i = 0; i < vals.Count - 1; i++)
                {
                    Process pre_sjf = new Process();
                    pre_sjf.burst_time = vals[i + 1].arrival_time - vals[i].arrival_time;
                    pre_sjf.Pid = vals[indx].Pid;
                    pre_sjf.arrival_time = vals[indx].arrival_time;
                    premtive_vals.Add(pre_sjf);
                    vals[indx].burst_time -= pre_sjf.burst_time;
                    int bst_tm = vals[indx].burst_time;
                    for (int j = i + 1; j >= 0; j--)
                        if (vals[j].burst_time < bst_tm) indx = j;
                }

                vals = vals.OrderBy(arr => arr.burst_time).ToList();
                for (int xx = 0; xx < vals.Count; xx++) premtive_vals.Add(vals.ElementAt(xx));//join lists
                for (int jj = 0; jj < premtive_vals.Count - 1; jj++)//join similar PID
                {
                    if (premtive_vals[jj].Pid == premtive_vals[jj + 1].Pid)
                    {
                        premtive_vals[jj + 1].burst_time += premtive_vals[jj].burst_time;
                        premtive_vals.RemoveAt(jj);
                        jj = -1;
                    }
                }

                int st_t = 0; int bs = 0;
                for (int j = 0; j < premtive_vals.Count; j++)
                {
                    //gantt chart code
                    ///////// type time lable
                    Label num_fcfs = new Label();
                    num_fcfs.Location = new Point(10 + st_t * 10, 305);
                    num_fcfs.Text = st_t.ToString();
                    if (premtive_vals[j].burst_time < 3) num_fcfs.Width = 28;
                    else num_fcfs.Width = premtive_vals[j].burst_time * 10 - 2;
                    flowLayoutPanel2.Controls.Add(num_fcfs);
                    /////////
                    st_t += premtive_vals[j].burst_time;
                    //// type process number lable
                    Label tempLabel_fcfs = new Label();
                    tempLabel_fcfs.Enabled = true;
                    tempLabel_fcfs.BorderStyle = BorderStyle.FixedSingle;
                    tempLabel_fcfs.Font = new Font("Arial", 10, FontStyle.Bold);
                    tempLabel_fcfs.Location = new Point(10 + time_fcfs * 10, 300);
                    string s = "P" + premtive_vals[j].Pid.ToString();
                    tempLabel_fcfs.Text = s;
                    tempLabel_fcfs.TextAlign = ContentAlignment.MiddleCenter;
                    if (premtive_vals[j].burst_time < 3) tempLabel_fcfs.Width = 30;
                    else tempLabel_fcfs.Width = premtive_vals[j].burst_time * 10;
                    flowLayoutPanel1.Controls.Add(tempLabel_fcfs);
                    ////////////////////////

                    //cal average waiting time
                    for (int z = j - 1; z >= 0; z--)
                        if (premtive_vals[j].Pid == premtive_vals[z].Pid)
                            bs = premtive_vals[z].burst_time;
                    prsjaverageWtime += (prsjwtime - bs - premtive_vals[j].arrival_time);
                    bs = 0;
                    prsjwtime += premtive_vals[j].burst_time;
                }

                //print last timelable in gant
                Label num_fcfss = new Label();
                num_fcfss.Location = new Point(10 + st_t * 10, 300);
                num_fcfss.Text = st_t.ToString();
                flowLayoutPanel2.Controls.Add(num_fcfss);
                /////////////////

                //print avg
                textBox1.Text = (prsjaverageWtime / (double)num_process).ToString() + " msec";
                vals.Clear();
                premtive_vals.Clear();
            }

        }
        class Process
        {
            public int Pid;
            public int burst_time;
            public int waiting_time = 0;
            public int last_active = 0;
            public int arrival_time;
            public int priority;

        }

        List<Process> vals = new List<Process>();

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        List<Process> premtive_vals = new List<Process>();
        int pd = 1;

        public int time_fcfs { get; private set; }
    }
}

