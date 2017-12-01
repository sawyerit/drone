using System.ComponentModel.Composition;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.MarketShare.Datasources;

namespace Drone.MarketShare.Components
{
	[Export(typeof(IDroneComponent))]
	public abstract class MarketShareBase<TComponentType> : BaseComponent<TComponentType>
	{

		#region constructors

		[ImportingConstructor]
		protected MarketShareBase()
			: base()
		{
			DroneDataSource = new MarketShareDataSource();
		}

		protected MarketShareBase(IDroneDataSource datasource)
			: base(datasource)
		{ }

		#endregion

		#region methods

		//private bool GetBoolFromXMLConfig(string node)//todo: is this used anymore?
		//{
		//	bool boolVal;
		//	bool.TryParse(XMLUtility.GetTextFromAccountNode(Xml, node), out boolVal);
		//	return boolVal;
		//}

		#endregion
	}
}
