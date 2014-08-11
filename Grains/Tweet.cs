using System.Reflection;
using System.Threading.Tasks;
using GrainInterfaces;
using log4net;
using Orleans;

namespace Grains
{
    public class Tweet : GrainBase, ITweet
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Task SetTweetInfo(string tweetText)
        {
            Log.DebugFormat("Grain tweet info set to {0}", tweetText);
            return TaskDone.Done;
        }
    }
}
