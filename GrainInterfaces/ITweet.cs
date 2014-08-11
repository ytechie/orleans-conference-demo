using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface ITweet : IGrain
    {
        Task SetTweetInfo(string tweetText);
    }
}
