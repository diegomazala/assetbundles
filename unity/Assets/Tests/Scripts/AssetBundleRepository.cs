using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleRepository : MonoBehaviour
{
    public string rootFolder = @"A:\";
    public string[] subFolders;
    private int currentSelected = 0;

    public UnityEngine.UI.InputField rootFolderInputField;
    public UnityEngine.UI.Dropdown dropDown;

    [System.Serializable]
    public class FolderSelectedEvent : UnityEngine.Events.UnityEvent<string>
    {
    }
    public FolderSelectedEvent OnFileSelected;

    void Start ()
    {
        SetupFileBrowser();
        dropDown.ClearOptions();

        var dirInfo = new System.IO.DirectoryInfo(rootFolder);

        if (dirInfo.Exists)
        {
            rootFolderInputField.text = rootFolder;

            subFolders = System.IO.Directory.GetDirectories(dirInfo.FullName);
            List<string> foldersName = new List<string>();

            foreach (var dir in subFolders)
            {
                foldersName.Add(new System.IO.DirectoryInfo(dir).Name);
            }

            dropDown.AddOptions(foldersName);
        }
	}

    public void OnFolderChanged(int index)
    {
        currentSelected = index;
        Debug.Log(subFolders[currentSelected]);
    }

    public string GetFolderSelected()
    {
        return subFolders[currentSelected];
    }


    void SetupFileBrowser()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        SimpleFileBrowser.FileBrowser.SetFilters(
            true,
            new SimpleFileBrowser.FileBrowser.Filter("Asset Bundles", ".unity3d"));


        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        SimpleFileBrowser.FileBrowser.SetDefaultFilter(".unity3d");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        SimpleFileBrowser.FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Icon: default (folder icon)
        // Name: Users
        // Path: C:\Users
        SimpleFileBrowser.FileBrowser.AddQuickLink(null, "Asset Drive", "A:\\");

        // Show a save file dialog 
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Initial path: "C:\", Title: "Save As", submit button text: "Save"
        // FileBrowser.ShowSaveDialog( null, null, false, "C:\\", "Save As", "Save" );



    }

    public void OnFileBrowserButtonPress()
    {
        // Coroutine example
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a select folder dialog 
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        // Load file/folder: folder, Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
        yield return SimpleFileBrowser.FileBrowser.ShowLoadDialog( (path) => { Debug.Log( "Selected: " + path ); }, 
                                        () => { Debug.Log( "Canceled" ); }, 
                                        true, null, "Select Folder", "Select" );

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(SimpleFileBrowser.FileBrowser.Success + " " + SimpleFileBrowser.FileBrowser.Result);

        rootFolderInputField.text = 
        rootFolder = SimpleFileBrowser.FileBrowser.Result;

        OnFileSelected.Invoke(rootFolder);
    }
}
