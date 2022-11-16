using System;
using System.Threading.Tasks;

namespace devoir
{
    public class ProgressViewer
    {
        public int progress;
        private readonly object counterLock = new object();
        public ProgressBar progressBar;

        public ProgressViewer()
        {
            progressBar = new ProgressBar();
        }

        public void UpdateProgressEvent(object sender, EventArgs e)
        {
            lock(counterLock)
            {
                progress++;
                progressBar.Report(((double) progress / 10020) * 100);
                // if (progress > 25 && progress < 50)
                // {
                //     Console.WriteLine("50%");
                // }
                // if (progress > 50 && progress < 100)
                // {
                //     Console.WriteLine("50%");
                // }
                //
                // if (progress > 99)
                // {
                //     Console.WriteLine("100%");
                // }
            }
            
        }


    }
}