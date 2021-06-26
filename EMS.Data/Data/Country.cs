using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace EMS.Data.Data
{
    public class Country : INotifyPropertyChanged
    {
        public string NAME { get; set; }
        private Visibility visibility = Visibility.Visible;
        public Visibility ItemsVisibility
        {
            get { return visibility; }
            set { visibility = value; }
        }
        private double weather { get; set; }
        public double Weather
        {
            get
            {
                return weather;
            }
            set
            {
                weather = value;
            }
        }
        private double population { get; set; }
        public double Population
        {
            get
            {
                return population;
            }
            set
            {
                population = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Population"));
            }
        }
        public string PopulationFormat { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PopulationFormat = (String.Format("{0:0,0}", this.Population).Trim('$'));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
    public class MapViewModel
    {
        public ObservableCollection<Country> Countries { get; set; }
        public MapViewModel()
        {
            Countries = new ObservableCollection<Country>();
            Countries = GetCountriesAndPopulation();
        }
        private ObservableCollection<Country> GetCountriesAndPopulation()
        {
            ObservableCollection<Country> countries = new ObservableCollection<Country>();
            countries.Add(new Country() { NAME = "China", Population = 1347350000 });
            countries.Add(new Country() { NAME = "United States", Population = 314623000 });
            countries.Add(new Country() { NAME = "Australia", Population = 22789701 });
            countries.Add(new Country() { NAME = "Russia", Population = 143228300 });
            countries.Add(new Country() { NAME = "Egypt", Population = 82724000 });
            countries.Add(new Country() { NAME = "South Africa", Population = 50586757 });
            return countries;
        }
    }
}