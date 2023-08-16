using ArthouseRepository.Data;
using ArthouseRepository.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ArthouseUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ArtistRepository _artistRepository;
        private readonly ArtworkRepository _artworkRepository;
        private readonly ArtTypeRepository _artTypeRepository;
        public MainPage()
        {
            this.InitializeComponent();
            _artistRepository = new ArtistRepository(App.ConnectionString);
            _artworkRepository = new ArtworkRepository(App.ConnectionString);
            _artTypeRepository = new ArtTypeRepository(App.ConnectionString);
            FillDropDowns();

        }

        private async void FillDropDowns()
        {
            //Show Progress
            progRing.IsActive = true;
            progRing.Visibility = Visibility.Visible;

            try
            {
                //Artists Lookup for Filter
                List<Lookup> artists = await _artistRepository.GetArtists();
                //Add the All Option
                artists.Insert(0, new Lookup { ID = 0, DisplayText = "- All Artists" });
                //Bind to the ComboBox
                ArtistCombo.ItemsSource = artists;

                //ArtType Lookup for Filter
                List<Lookup> artTypes = await _artTypeRepository.GetArtTypes();
                //Add the All Option
                artTypes.Insert(0, new Lookup { ID = 0, DisplayText = "- All Types" });
                //Bind to the ComboBox
                TypeOfArtCombo.ItemsSource = artTypes;
                //In case there previously was a filter
                txtTitleFilter.Text = "";

                ShowArtworks(null, null, null);
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                if (errMsg.Contains("Failed to generate SSPI context"))
                {
                    App.ShowMessage("Error", "You need to enable Enterprise Authentication in Capabilities.");
                }
                else if (errMsg.Contains("server"))
                {
                    App.ShowMessage("Error", "Could not connect to the database server. Are you using the correct connetion string?");
                }

                else if (errMsg.Contains("databse"))
                {
                    App.ShowMessage("Error", "Could not connect to the database server. Did you create the database?");
                }
                else
                {
                    App.ShowMessage("Error", "Could not complete operation");
                }
            }
            finally
            {
                progRing.IsActive = false;
                progRing.Visibility = Visibility.Collapsed;
            }
        }

        private async void ShowArtworks(int? ArtTypeID, int? ArtistID, string strTitleFilter)
        {
            //Show Progress
            progRing.IsActive = true;
            progRing.Visibility = Visibility.Visible;

            try
            {
                List<Artwork> artworks;

                if (ArtTypeID.GetValueOrDefault() > 0 ||
                    ArtistID.GetValueOrDefault() > 0 ||
                    !String.IsNullOrEmpty(strTitleFilter))
                {
                    artworks = await _artworkRepository.GetArtworksByX(ArtTypeID.GetValueOrDefault(),
                        ArtistID.GetValueOrDefault(), strTitleFilter);
                }
                else
                {
                    artworks = await _artworkRepository.GetArtworks();
                }

                artworkList.ItemsSource = artworks;

            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                if (errMsg.Contains("Failed to generate SSPI context"))
                {
                    App.ShowMessage("Error", "You need to enable Enterprise Authentication in Capabilities.");
                }
                else if (errMsg.Contains("server"))
                {
                    App.ShowMessage("Error", "Could not connect to the database server. Are you using the correct connetion string?");
                }

                else if (errMsg.Contains("databse"))
                {
                    App.ShowMessage("Error", "Could not connect to the database server. Did you create the database?");
                }
                else
                {
                    App.ShowMessage("Error", "Could not complete operation");
                }
            }
            finally
            {
                progRing.IsActive = false;
                progRing.Visibility = Visibility.Collapsed;
            }

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            FillDropDowns();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            Lookup selectedArtist = (Lookup)ArtistCombo.SelectedItem;
            Lookup selectedArtType = (Lookup)TypeOfArtCombo.SelectedItem;
            ShowArtworks(selectedArtType?.ID, selectedArtist?.ID, txtTitleFilter.Text);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ArtistCombo.SelectedIndex = -1;
            TypeOfArtCombo.SelectedIndex = -1;
            txtTitleFilter.Text = "";
            ShowArtworks(null, null, null);
        }

        private void artworkGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the detail page
            Frame.Navigate(typeof(ArtworkDetailPage), (Artwork)e.ClickedItem);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Artwork newArtwork = new Artwork();
            newArtwork.DateFinished = DateTime.Now;//Avoid NULL issues
            // Navigate to the detail page
            Frame.Navigate(typeof(ArtworkDetailPage), newArtwork);
        }
    }
}
