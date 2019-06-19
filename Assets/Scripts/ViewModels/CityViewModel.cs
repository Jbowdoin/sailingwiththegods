using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

public class CityViewModel : Model
{
	private GameVars GameVars;
	private Settlement City;

	public readonly ObservableCollection<CrewManagementMemberViewModel> Crew;
	public readonly ObservableCollection<CargoInventoryViewModel> Buy;
	public readonly ObservableCollection<CargoInventoryViewModel> Sell;

	public string PortName => City.name;

	public CityViewModel(Settlement city) {
		GameVars = Globals.GameVars;
		City = city;

		Crew = new ObservableCollection<CrewManagementMemberViewModel>(
			GameVars.Network.CrewMembersWithNetwork(city)
				.Select(crew => new CrewManagementMemberViewModel(crew, null))
				.OrderBy(c => GameVars.Network.GetCrewMemberNetwork(c.Member).Count())
				.Take(5)
		);

		var priceInfo = City.cargo
				.Select(resource => new {
					Resource = resource,
					Price = GameVars.Trade.GetPriceOfResource(resource.name, City),
					AvgPrice = GameVars.Trade.GetAvgPriceOfResource(resource.name)
				});

		Buy = new ObservableCollection<CargoInventoryViewModel>(
			priceInfo
				.OrderBy(o => o.Price - o.AvgPrice)
				.Select(o => new CargoInventoryViewModel(o.Resource))
				.Take(5)
		);

		Sell = new ObservableCollection<CargoInventoryViewModel>(
			priceInfo
				.OrderByDescending(o => o.Price - o.AvgPrice)
				.Select(o => new CargoInventoryViewModel(o.Resource))
				.Take(5)
		);
	}
}