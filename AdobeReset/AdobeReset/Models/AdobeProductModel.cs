using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using AdobeReset.Structs;

namespace AdobeReset.Models
{
    public class AdobeProductModel : ViewModelBase
    {
        private ObservableCollection<AdobeProduct> _adobeProducts;

        public AdobeProductModel()
        {
            _adobeProducts = new ObservableCollection<AdobeProduct>();
        }

        public ObservableCollection<AdobeProduct> AdobeProducts
        {
            get => _adobeProducts;
            set {
                if (_adobeProducts.Equals(value)) {
                    return;
                }

                _adobeProducts = value;
                OnPropertyChanged();
            }
        }

        public void SetCollection(ConcurrentBag<AdobeProduct> productList)
        {
            this.AdobeProducts = new ObservableCollection<AdobeProduct>(productList);
        }
    }
}
