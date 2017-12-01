using Drone.Entities.Crunchbase;
using Drone.Core;

namespace Drone.Entities.Crunchbase
{
	public class CrunchbaseDataComponent : BaseDataComponent<CrunchbaseComponent>
	{
		public CompanyRoot CompanyLocal { get; set; }
	}
}
