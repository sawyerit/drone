namespace Drone.API.MarketAnalysis
{
	public interface IProcessor
	{
		bool Process(DOMReader dom, MarketShareRule rule);
	}
}
