using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace MovieTexture2AssetEditorPlugin
{
    public class MovieTexture2AssetDefition : AssetDefinition
    {

        protected static ImageSource iconSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;Component/Images/Assets/MovieTextureFileType.png") as ImageSource;

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new MovieTexture2Editor(logger);
        }

        public override ImageSource GetIcon()
        {
            return iconSource;
        }
    }

    public class MovieTexture2Editor : FrostyAssetEditor
    {
        public MovieTexture2Editor(ILogger logger) : base(logger)
        {
        }

        public override List<ToolbarItem> RegisterToolbarItems() {
            return new List<ToolbarItem>
            {
                new ToolbarItem("Export", "Export Texture", "Images/Export.png", new RelayCommand((object state) => { ExportButton_Click(this, new RoutedEventArgs()); })),
                new ToolbarItem("Import", "Import Texture", "Images/Import.png", new RelayCommand((object state) => { ImportButton_Click(this, new RoutedEventArgs()); })),
            };
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic root = asset.RootObject;
            ChunkAssetEntry chunkAssetEntry = App.AssetManager.GetChunkEntry(root.ChunkGuid);

            FrostySaveFileDialog saveFileDialog = new FrostySaveFileDialog("Export Movie Asset", "WEBM (*.webm)|*.webm", "Movie", AssetEntry.Filename, false);
            bool result = false;
            while (true)
            {
                string initialDir = saveFileDialog.InitialDirectory;
                result = saveFileDialog.ShowDialog();

                if (result)
                {
                    FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
                    saveFileDialog.InitialDirectory = fileInfo.DirectoryName;

                    if (fileInfo.Exists)
                    {
                        if (FrostyMessageBox.Show(saveFileDialog.FileName + " already exists\r\nDo you want to replace it?", "Frosty Editor (Exporting Movie Asset)", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!result)
            {
                return;
            }

            FrostyTaskWindow.Show("Exporting video", "Exporting video...", (task) =>
            {
                Stream chunkStream = App.AssetManager.GetChunk(chunkAssetEntry);
                if (chunkStream != null)
                {
                    using (NativeWriter writer = new NativeWriter(new FileStream(saveFileDialog.FileName, FileMode.Create)))
                    {
                        using (NativeReader reader = new NativeReader(chunkStream))
                            writer.Write(reader.ReadToEnd());
                    }
                }
                else
                {
                    logger.LogError("Failed to export chunk ${chunkAssetEntry.ChunkGuid}. Maybe it doesn't exist?");
                }
            });
            logger.Log("Exported Movie Asset to " + saveFileDialog.FileName);
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement import functionality here
            logger.Log("Import button clicked");
        }
    }
}
