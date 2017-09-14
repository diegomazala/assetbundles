using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AssetBundleSelector : MonoBehaviour
{
    private int currentSelected = 0;

    public UnityEngine.UI.InputField rootFolderInputField;

    public string AssetBundleFileName { get; protected set; }



    [System.Serializable]
    public class FileNameEvent : UnityEngine.Events.UnityEvent<string>
    {
    }
    public FileNameEvent OnFileSelected;



    void Start ()
    {
        SetupFileBrowser();
        rootFolderInputField.text = "";
    }

    void SetupFileBrowser()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        SimpleFileBrowser.FileBrowser.SetFilters(
            true,
            new SimpleFileBrowser.FileBrowser.Filter("Unity Bundles", ".unity3d"),
            new SimpleFileBrowser.FileBrowser.Filter("Scene Bundles", ".scene"),
            new SimpleFileBrowser.FileBrowser.Filter("Game Object Bundles", ".gobj"));


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
        //SimpleFileBrowser.FileBrowser.AddQuickLink(null, "Asset Drive", "A:\\");

        // Show a save file dialog 
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Initial path: "C:\", Title: "Save As", submit button text: "Save"
        // FileBrowser.ShowSaveDialog( null, null, false, "C:\\", "Save As", "Save" );

        // Show a select folder dialog 
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        // Load file/folder: folder, Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
        // FileBrowser.ShowLoadDialog( (path) => { Debug.Log( "Selected: " + path ); }, 
        //                                () => { Debug.Log( "Canceled" ); }, 
        //                                true, null, "Select Folder", "Select" );

    }

    public void OnFileBrowserButtonPress()
    {
        // Coroutine example
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return SimpleFileBrowser.FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(SimpleFileBrowser.FileBrowser.Success + " " + SimpleFileBrowser.FileBrowser.Result);

        AssetBundleFileName = rootFolderInputField.text = SimpleFileBrowser.FileBrowser.Result;
        Debug.Log(AssetBundleFileName);

        OnFileSelected.Invoke(SimpleFileBrowser.FileBrowser.Result);
    }
}
