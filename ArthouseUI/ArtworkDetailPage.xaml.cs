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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ArthouseUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtworkDetailPage : Page
    {
        //The View for the page is a single Artwork
        Artwork view;

        //Data Repositories
        private readonly ArtistRepository _artistRepository;
        private readonly ArtworkRepository _artworkRepository;
        private readonly ArtTypeRepository _artTypeRepository;

        //To know if we are Inserting or Updating
        bool InsertMode;
        public ArtworkDetailPage()
        {
            this.InitializeComponent();
            _artistRepository = new ArtistRepository(App.ConnectionString);
            _artworkRepository = new ArtworkRepository(App.ConnectionString);
            _artTypeRepository = new ArtTypeRepository(App.ConnectionString);
            fillDropDowns();
        }

        private async void fillDropDowns()
        {
            try
            {
                //Get the Lookup Values
                List<Lookup> artists = await _artistRepository.GetArtists();
                List<Lookup> artTypes = await _artTypeRepository.GetArtTypes();
                //Bind to the ComboBoxes
                ArtistCombo.ItemsSource = artists;
                ArtTypeCombo.ItemsSource = artTypes;

                //Now you can assign the DataContext for the page
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                if (errMsg.Contains("Failed to generate SSPI context"))
                {
                    MessageDialog d = new MessageDialog("You need to enable Enterprise Authentication in Capabilities.", "Error");
                    await d.ShowAsync();
                }
                else if (errMsg.Contains("server"))
                {
                    MessageDialog d = new MessageDialog("Could not connect to the database server. Are you using the correct connetion string?", "Error");
                    await d.ShowAsync();
                }

                else if (errMsg.Contains("database"))
                {
                    MessageDialog d = new MessageDialog("Could not connect to the database server. Did you create the database?", "Error");
                    await d.ShowAsync();
                }
                else
                {
                    MessageDialog d = new MessageDialog("Could not complete operation.", "Error");
                    await d.ShowAsync();
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            view = (Artwork)e.Parameter;

            if (view.ID == 0) //Inserting
            {
                //Disable the delete button if adding
                btnDelete.IsEnabled = false;
                InsertMode = true;
            }
            else
            {
                InsertMode = false;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsValid())
                {
                    if (InsertMode)
                    {
                        int recordsAffected = await _artworkRepository.AddArtwork(view);
                        if (recordsAffected == 0)
                        {
                            MessageDialog d = new MessageDialog("Could not add " + view.Summary, "Error");
                            await d.ShowAsync();
                        }
                    }
                    else
                    {
                        int recordsAffected = await _artworkRepository.UpdateArtwork(view);
                        if (recordsAffected == 0)
                        {
                            MessageDialog d = new MessageDialog("Could not update " + view.Summary, "Error");
                            await d.ShowAsync();
                        }
                    }
                    Frame.GoBack();
                }

            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                if (errMsg.Contains("Failed to generate SSPI context"))
                {
                    MessageDialog d = new MessageDialog("You need to enable Enterprise Authentication in Capabilities.", "Error");
                    await d.ShowAsync();
                }
                else if (errMsg.Contains("server"))
                {
                    MessageDialog d = new MessageDialog("Could not connect to the database server. Are you using the correct connetion string?", "Error");
                    await d.ShowAsync();
                }

                else if (errMsg.Contains("database"))
                {
                    MessageDialog d = new MessageDialog("Could not connect to the database server. Did you create the database?", "Error");
                    await d.ShowAsync();
                }
                else
                {
                    MessageDialog d = new MessageDialog("Could not complete operation.", "Error");
                    await d.ShowAsync();
                }
            }
        }

        private bool IsValid()
        {
            //Start by assuming everything is good
            bool valid = true;
            string message = "Please fix the following errors:\n\n";

            //Check each requirement and add to the message.
            if (string.IsNullOrEmpty(view.Title))
            {
                valid = false;
                message += "You must enter the Title for the Artwork. \n";
            }
            if (view.ArtistID == 0)
            {
                valid = false;
                message += "You must select the Artist. \n";
            }
            if (view.ArtTypeID == 0)
            {
                valid = false;
                message += "You must select the Type of Art. \n";
            }
            if (view.Value < 0)
            {
                valid = false;
                message += "The Value of the Artwork cannot be negative. \n";
            }
            if (string.IsNullOrEmpty(view.Description))
            {
                valid = false;
                message += "A decription for the Artwork is required. \n";
            }
            //Show the meessage if needed
            if (!valid)
            {
                App.ShowMessage("Error", message);

            }
            return valid;
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string strTitle = "Confirm Delete";
            string strMsg = "Are you certain that you want to delete " + view.Summary + "?";
            ContentDialogResult choice = await App.ConfirmDialog(strTitle, strMsg);
            if (choice == ContentDialogResult.Secondary)
            {
                try
                {
                    int result = await _artworkRepository.DeleteArtwork(view);
                    if (result == 0)
                    {
                        MessageDialog d = new MessageDialog("Could not delete " + view.Summary, "Error");
                        await d.ShowAsync();
                    }
                    Frame.GoBack();
                }
                catch (Exception ex)
                {
                    string errMsg = ex.GetBaseException().Message;
                    if (errMsg.Contains("Failed to generate SSPI context"))
                    {
                        MessageDialog d = new MessageDialog("You need to enable Enterprise Authentication in Capabilities.", "Error");
                        await d.ShowAsync();
                    }
                    else if (errMsg.Contains("server"))
                    {
                        MessageDialog d = new MessageDialog("Could not connect to the database server. Are you using the correct connetion string?", "Error");
                        await d.ShowAsync();
                    }

                    else if (errMsg.Contains("database"))
                    {
                        MessageDialog d = new MessageDialog("Could not connect to the database server. Did you create the database?", "Error");
                        await d.ShowAsync();
                    }
                    else
                    {
                        MessageDialog d = new MessageDialog("Could not complete operation.", "Error");
                        await d.ShowAsync();
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
