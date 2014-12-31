using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.IO;

namespace RenameSubTitle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> videoFiles;
        public List<string> subFiles;
        public List<string> videoFullPath;
        public List<string> subFullPath;
        public string currentDirectory { get; set; }
        private ObservableCollection<string> ListItemsVideo = new ObservableCollection<string>();
        private ObservableCollection<string> ListItemsSub = new ObservableCollection<string>();
        BindingClass binding = new BindingClass();
        FrameworkElement fe = new FrameworkElement();

        public MainWindow()
        {
            InitializeComponent();
            Initialize();            
        }

        public void Initialize()
        {
            videoFiles = new List<string>();
            subFiles = new List<string>();
            videoFullPath = new List<string>();
            subFullPath = new List<string>();
            currentDirectory = Directory.GetCurrentDirectory();            
            lbVideo.ItemsSource = this.ListItemsVideo;
            lbSub.ItemsSource = this.ListItemsSub;
            LoadVideo(currentDirectory);
            LoadSub(currentDirectory);
            InitializeContextMenu();
        }

        private void InitializeContextMenu()
        {            
            var miVideo = new System.Windows.Controls.MenuItem();
            miVideo.Header = "Delete All";
            miVideo.Click += mi_Click;
            fe.ContextMenu = new System.Windows.Controls.ContextMenu();
            fe.ContextMenu.Items.Add(miVideo);
            lbVideo.ContextMenu = fe.ContextMenu;

            var miSub = new System.Windows.Controls.MenuItem();
            miSub.Header = "Delete All";
            miSub.Click += miVideo_Click;
            fe.ContextMenu = new System.Windows.Controls.ContextMenu();
            fe.ContextMenu.Items.Add(miSub);
            lbSub.ContextMenu = fe.ContextMenu;            
        }        

        private void btLoadVideo_Click(object sender, RoutedEventArgs e)
        {            
            LoadVideo("", true, true);
        }

        private void btLoadSub_Click(object sender, RoutedEventArgs e)
        {            
            LoadSub("", true, true);
        }

        public void LoadVideo(string directory, bool add = false, bool isDirectory = false)
        {
            if (!add)
            {
                videoFullPath.Clear();
                videoFiles.Clear();
            }
            if (isDirectory)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    directory = dialog.SelectedPath;
            }
            if (directory != string.Empty)
            {
                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).
                    Where(f => f.ToLower().EndsWith(".avi") || f.ToLower().EndsWith(".mp4") || f.ToLower().EndsWith(".mkv")).ToList();                 
                
                foreach (string name in files)
                {
                    videoFullPath.Add(name);
                    videoFiles.Add(System.IO.Path.GetFileName(name));
                }
            }
            else
            {                
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = "AVI files(.avi);MP4 files(.mp4);MKV files(.mkv)|*.avi;*.mp4;*.mkv|All Files|*.*";
                dialog.FilterIndex = 1;
                dialog.ShowDialog();                
                foreach (string name in dialog.FileNames)
                {
                    videoFullPath.Add(name);
                    videoFiles.Add(System.IO.Path.GetFileName(name));                    
                }
            }
            RefreshVideoFiles();
        }

        public void LoadSub(string directory, bool add = false, bool isDirectory = false)
        {
            if (!add)
            {
                subFullPath.Clear();
                subFiles.Clear();
            }
            if (isDirectory)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    directory = dialog.SelectedPath;
            }
            if (directory != string.Empty)
            {
                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).
                    Where(f => f.ToLower().EndsWith(".sub") || f.ToLower().EndsWith(".srt")).ToList();
                foreach (string name in files)
                {
                    subFullPath.Add(name);
                    subFiles.Add(System.IO.Path.GetFileName(name));
                }
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = "Subtitle files(.sub);SRT subtitle files(.srt)|*.srt;*.sub|All Files|*.*";
                dialog.ShowDialog();
                foreach (string name in dialog.FileNames)
                {
                    subFullPath.Add(name);
                    subFiles.Add(System.IO.Path.GetFileName(name));
                    //ListItemsSub.Add(System.IO.Path.GetFileName(name));
                }
            }
            RefreshSubFiles();
        }

        private void RefreshVideoFiles()
        {
            ListItemsVideo.Clear();
            foreach (string vfile in videoFiles)
                ListItemsVideo.Add(vfile);
        }

        private void RefreshSubFiles()
        {
            ListItemsSub.Clear();
            foreach (string sfile in subFiles)
                ListItemsSub.Add(sfile);
        }

        public List<string> SplitFileNames(List<string> fileNames)
        {            
            List<string> splitted = new List<string>();
            string concetanater = string.Empty;
            List<string> buffer = new List<string>(); 

            foreach (string name in fileNames)
            {
                concetanater = string.Empty;
                buffer.Clear();               
                foreach (string _name in name.Split('.'))
                {
                    buffer.Add(_name);
                }
                // Kutyiiii                
                for (int i = 0; i < buffer.Count-1; i++)
                {
                    if (i == buffer.Count - 2)
                        concetanater += buffer[i];                      
                    else concetanater += buffer[i] + ".";                      
                }                
                splitted.Add(concetanater);                 
            }
            return splitted;
        }

        private void btRename_Click(object sender, RoutedEventArgs e)
        {
            string showName = string.Empty;
            string dirName = string.Empty;
            List<string> name = new List<string>();
            var sb = new StringBuilder();
            var modifiableSubsCount = subFullPath.Count;

            binding.ShowAgain = false;
            name = SplitFileNames(videoFiles);
            sb.AppendLine("The following files will be modified:");
            for (var i = 0; i < subFullPath.Count; i++)
            {
                dirName = System.IO.Path.GetDirectoryName(subFullPath[i]);                
                var equal = Directory.GetFiles(dirName).ToList().Any(f => f == (dirName + "\\" + name[i] + ".srt"));
                if (File.Exists(dirName + "\\" + name[i] + ".srt") && equal)
                {
                    modifiableSubsCount--;
                    if (binding.ShowAgain == true)
                    {
                        DialogBox dialog = new DialogBox("Ilyen feliratfájl már létezik ezzel a névvel:\n" + dirName + "\\" + name[i] + ".srt");
                        dialog.DataContext = binding;
                        dialog.ShowDialog();
                    }                    
                    continue;
                }
                else
                    sb.AppendLine(subFullPath[i] + " -> " + dirName + "\\" + name[i] + ".srt");
            }
            if (modifiableSubsCount > 0)
            {
                var result = System.Windows.MessageBox.Show(sb.ToString() + "\nDo you continue?", "File modification",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result != MessageBoxResult.OK)
                    return;
            }
            else
            {
                System.Windows.MessageBox.Show("There aren't any modifiable subtitle!", "No more subtitle", MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                return;
            }
            for (var i = 0; i < subFullPath.Count; i++)
            {
                dirName = System.IO.Path.GetDirectoryName(subFullPath[i]);
                var equal = Directory.GetFiles(dirName).ToList().Any(f => f == (dirName + "\\" + name[i] + ".srt"));
                if (!File.Exists(dirName + "\\" + name[i] + ".srt") || !equal)
                    File.Move(subFullPath[i], dirName + "\\" + name[i] + ".srt");   
            }
            var count = videoFullPath.Count <= subFullPath.Count ? videoFullPath.Count : subFullPath.Count;
            for (var i = count-1; i >= 0; i--)            
                DeleteSubtitle(i);            
            System.Threading.Thread.Sleep(1000);
            if (currentDirectory != string.Empty)
            {
                //LoadVideo(currentDirectory);
                LoadSub(currentDirectory);                
            }            
        }

        private void btVideoDown_Click(object sender, RoutedEventArgs e)
        {            
            var selectedIndex = this.lbVideo.SelectedIndex;

            if (selectedIndex + 1 < this.ListItemsVideo.Count && selectedIndex != -1)
            {
                var itemToMoveDown = this.ListItemsVideo[selectedIndex];
                this.ListItemsVideo.RemoveAt(selectedIndex);
                this.ListItemsVideo.Insert(selectedIndex + 1, itemToMoveDown);
                this.lbVideo.SelectedIndex = selectedIndex + 1;

                itemToMoveDown = videoFullPath[selectedIndex].ToString();
                videoFullPath.RemoveAt(selectedIndex);
                videoFullPath.Insert(selectedIndex + 1, itemToMoveDown);

                itemToMoveDown = videoFiles[selectedIndex].ToString();
                videoFiles.RemoveAt(selectedIndex);
                videoFiles.Insert(selectedIndex + 1, itemToMoveDown);
            }
        }

        private void btSubDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.lbSub.SelectedIndex;

            if (selectedIndex + 1 < this.ListItemsSub.Count && selectedIndex != -1)
            {
                var itemToMoveDown = this.ListItemsSub[selectedIndex];
                this.ListItemsSub.RemoveAt(selectedIndex);
                this.ListItemsSub.Insert(selectedIndex + 1, itemToMoveDown);
                this.lbSub.SelectedIndex = selectedIndex + 1;
                
                itemToMoveDown = subFullPath[selectedIndex].ToString();
                subFullPath.RemoveAt(selectedIndex);
                subFullPath.Insert(selectedIndex + 1, itemToMoveDown);

                itemToMoveDown = subFiles[selectedIndex].ToString();
                subFiles.RemoveAt(selectedIndex);
                subFiles.Insert(selectedIndex + 1, itemToMoveDown); 
            }
        }

        private void btVideoUp_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.lbVideo.SelectedIndex;

            if (selectedIndex > 0)
            {
                var itemToMoveUp = this.ListItemsVideo[selectedIndex];
                this.ListItemsVideo.RemoveAt(selectedIndex);
                this.ListItemsVideo.Insert(selectedIndex - 1, itemToMoveUp);
                this.lbVideo.SelectedIndex = selectedIndex - 1;

                itemToMoveUp = videoFullPath[selectedIndex].ToString();
                videoFullPath.RemoveAt(selectedIndex);
                videoFullPath.Insert(selectedIndex - 1, itemToMoveUp);

                itemToMoveUp = videoFiles[selectedIndex].ToString();
                videoFiles.RemoveAt(selectedIndex);
                videoFiles.Insert(selectedIndex - 1, itemToMoveUp);
            }
        }

        private void btSubUp_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.lbSub.SelectedIndex;

            if (selectedIndex > 0)
            {
                var itemToMoveUp = this.ListItemsSub[selectedIndex];
                this.ListItemsSub.RemoveAt(selectedIndex);
                this.ListItemsSub.Insert(selectedIndex - 1, itemToMoveUp);
                this.lbSub.SelectedIndex = selectedIndex - 1;

                itemToMoveUp = subFullPath[selectedIndex].ToString();
                subFullPath.RemoveAt(selectedIndex);
                subFullPath.Insert(selectedIndex - 1, itemToMoveUp);

                itemToMoveUp = subFiles[selectedIndex].ToString();
                subFiles.RemoveAt(selectedIndex);
                subFiles.Insert(selectedIndex - 1, itemToMoveUp); 
            }
        }

        private void lbVideo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {                
                var selectedIndex = this.lbVideo.SelectedIndex;
                if (selectedIndex == -1)
                {
                    System.Windows.MessageBox.Show("Nincs kiválasztva fájl!", "Nincs kiválasztva", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                DeleteVideo(selectedIndex);                
            }
        }

        private void DeleteVideo(int selectedIndex)
        {
            this.ListItemsVideo.RemoveAt(selectedIndex);
            if (this.ListItemsVideo.Count > 0)
                if (selectedIndex == this.ListItemsVideo.Count)
                    this.lbVideo.SelectedIndex = selectedIndex-1;
                else this.lbVideo.SelectedIndex = selectedIndex;
            videoFullPath.RemoveAt(selectedIndex);
            videoFiles.RemoveAt(selectedIndex); 
        }

        private void lbSub_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedIndex = this.lbSub.SelectedIndex;
                if (selectedIndex == -1)
                {
                    System.Windows.MessageBox.Show("Nincs kiválasztva fájl!", "Nincs kiválasztva", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                DeleteSubtitle(selectedIndex);                
            }
        }

        private void DeleteSubtitle(int selectedIndex)
        {
            this.ListItemsSub.RemoveAt(selectedIndex);
            if (this.ListItemsSub.Count > 0)
                if (selectedIndex == this.ListItemsSub.Count)
                    this.lbSub.SelectedIndex = selectedIndex-1;
                else this.lbSub.SelectedIndex = selectedIndex;
            subFullPath.RemoveAt(selectedIndex);
            subFiles.RemoveAt(selectedIndex);
        }

        private void btAddVideo_Click(object sender, RoutedEventArgs e)
        {
            LoadVideo("", true);
        }

        private void btAddSub_Click(object sender, RoutedEventArgs e)
        {
            LoadSub("", true);
        }                

        void mi_Click(object sender, RoutedEventArgs e)
        {
            for (int i = this.ListItemsVideo.Count-1; i >= 0; i--)
            {
                DeleteVideo(i);
            }    
        }

        void miVideo_Click(object sender, RoutedEventArgs e)
        {
            for (int i = this.ListItemsSub.Count - 1; i >= 0; i--)
            {
                DeleteSubtitle(i);
            }  
        }
    }
}
