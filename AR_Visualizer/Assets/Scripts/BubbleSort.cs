/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using System;

public class CubeGenerator : MonoBehaviour
{
    public ARPlaneManager arPlaneManager; // Reference to the ARPlaneManager
    public GameObject cubePrefab;
    public GameObject indexPrefab;
    public TMP_InputField userInputField;
    public Button submitButton;
    public float spacing = 1f;
    public Color textColor = Color.white;
    public Color indexColor = Color.black;
    public Color comparisonColor = Color.yellow;
    public Color indcompColor = Color.red;
    public Color sortedColor = Color.green;
    public float sortingDelay = 1f;
    public float swapSpeed = 12f;
    public GameObject bubbleInputCanvas; // Reference to the BubbleInputCanvas
    public Camera mainCamera;
    public GameObject proAnchor;

    public TextMeshProUGUI iterationText;
    private GameObject[] cubes;
    private GameObject[] indexes;
    private bool sortingInProgress = false;
    private bool paused = false;
    public GameObject placeOnPlaneObject;
    private void Start()
    {
        // submitButton.onClick.AddListener(OnSubmitButtonClick);
    }
    public void OnSubmitButtonClick()
    {
        // This method will be called when the submit button is clicked
        // Generate cubes only if a plane is detected
        // Assuming you want to use the first detected plane
        bubbleInputCanvas.SetActive(false);

        ARPlane plane = null;
        foreach (var trackable in arPlaneManager.trackables)
        {
            if (trackable is ARPlane arPlane)
            {
                plane = arPlane;
                break;
            }
        }

        if (plane != null)
        {
            GenerateCubes();
        }
        else
        {
            Debug.LogError("No AR planes detected.");
        }
    }
    public void GenerateCubes()
{


    if ( sortingInProgress || cubes != null)
    {
        return;
    }

    // Your existing cube generation logic goes here...
    bubbleInputCanvas.SetActive(false);
    sortingInProgress = true;

    // Hide the BubbleInputCanvas

    // Destroy previous cubes and indexes
    DestroyCubesAndIndexes();

    string userInput = userInputField.text;
    string[] numbers = userInput.Split(',');

    // Calculate total width
    float totalWidth = (numbers.Length - 1) * spacing;
    float startX = -totalWidth / 2f;
    Vector3 iterationTextPosition = new Vector3(startX - 0.3f, 0f, 0f);

    float currentX = startX;

    cubes = new GameObject[numbers.Length];
    indexes = new GameObject[numbers.Length];
     Vector3 initialcubePosition = new Vector3(currentX, 0f, 0f);
        GameObject inicube = Instantiate(cubePrefab, initialcubePosition, Quaternion.identity, proAnchor.transform);

        for (int i = 0; i < numbers.Length; i++)
    {
        Vector3 cubePosition = new Vector3(currentX, 0f, 0f);
        Vector3 indexPosition = new Vector3(currentX, -100f, 0f); // Adjust position relative to the plane

        // Instantiate cube and index with ProductAnchor as parent
        GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity, proAnchor.transform);
        GameObject index = Instantiate(indexPrefab, indexPosition, Quaternion.identity, proAnchor.transform);

        currentX += spacing*0.5f;

        cubes[i] = cube;
        indexes[i] = index;

        // Set up cube and index UI
        SetupCubeAndIndexUI(cube, index, numbers[i], i);
    }

    StartCoroutine(BubbleSortCoroutine());

    // Focus main camera on the cubes
    //FocusMainCameraOnCubes();
}



    // Focus main camera on the cubes
    //FocusMainCameraOnCubes();





    // public void GenerateCubesOnPlane(ARPlane plane)
    // {
    //     if (sortingInProgress)
    //     {
    //         return;
    //     }

    //     sortingInProgress = true;

    //     // Hide the BubbleInputCanvas
    //     bubbleInputCanvas.SetActive(false);

    //     // Destroy previous cubes and indexes
    //     DestroyCubesAndIndexes();

    //     string userInput = userInputField.text;
    //     string[] numbers = userInput.Split(',');

    //     // Calculate total width
    //     float totalWidth = (numbers.Length - 1) * spacing;
    //     float startX = -totalWidth / 2f;
    //     Vector3 iterationTextPosition = new Vector3(startX - 0.3f, 0f, 0f);

    //     float currentX = startX;

    //     cubes = new GameObject[numbers.Length];
    //     indexes = new GameObject[numbers.Length];

    //     // Get the local position and rotation of the plane
    //     Vector3 planePosition = plane.transform.localPosition;
    //     Quaternion planeRotation = plane.transform.localRotation;

    //     for (int i = 0; i < numbers.Length; i++)
    //     {
    //         // Calculate the position relative to the plane's local coordinate system
    //         Vector3 cubeLocalPosition = new Vector3(currentX, 0f, 0f);
    //         Vector3 indexLocalPosition = new Vector3(currentX, -100f, 0f);

    //         // Transform the local positions to world positions
    //         Vector3 cubePosition = plane.transform.TransformPoint(cubeLocalPosition);
    //         Vector3 indexPosition = plane.transform.TransformPoint(indexLocalPosition);

    //         GameObject cube = Instantiate(cubePrefab, cubePosition, planeRotation, plane.transform); // Set plane as parent
    //         GameObject index = Instantiate(indexPrefab, indexPosition, planeRotation, plane.transform); // Set plane as parent

    //         currentX += spacing;

    //         cubes[i] = cube;
    //         indexes[i] = index;

    //         // Set up cube and index UI
    //         SetupCubeAndIndexUI(cube, index, numbers[i], i);
    //     }

    //     // Start sorting coroutine
    //     StartCoroutine(BubbleSortCoroutine());
    // }

    private void DestroyCubesAndIndexes()
    {
        if (cubes != null)
        {
            foreach (GameObject cube in cubes)
            {
                Destroy(cube);
            }
        }

        if (indexes != null)
        {
            foreach (GameObject index in indexes)
            {
                Destroy(index);
            }
        }
    }

    private void SetupCubeAndIndexUI(GameObject cube, GameObject index, string number, int indexNumber)
    {
        Canvas canvas = cube.GetComponentInChildren<Canvas>();
        Canvas indexCanvas = index.GetComponentInChildren<Canvas>();

        if (canvas != null && indexCanvas != null)
        {
            TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI indexTextMesh = indexCanvas.GetComponentInChildren<TextMeshProUGUI>();

            if (textMesh != null && indexTextMesh != null)
            {
                textMesh.text = number;
                textMesh.color = textColor;
                textMesh.alignment = TextAlignmentOptions.Center;

                indexTextMesh.text = indexNumber.ToString();
                indexTextMesh.color = indexColor;
                indexTextMesh.alignment = TextAlignmentOptions.Center;

                float cubeSize = 24.2f;
                float fontSizeMultiplier = 4f;
                textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
                indexTextMesh.fontSize = Mathf.RoundToInt(cubeSize * 4f);
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube or index prefab.");
            }
        }
        else
        {
            Debug.LogError("Canvas component not found in the children of the cube or index prefab.");
        }
    }

    private IEnumerator BubbleSortCoroutine()
    {
        yield return new WaitForSeconds(sortingDelay);

        int n = cubes.Length;

        for (int iteration = 1; iteration < n; iteration++)
        {
            bool swapped = false;

            if (iterationText != null)
            {
                iterationText.text = "Iteration: " + iteration;
            }

            for (int i = 1; i < n; i++)
            {
                indexes[i].GetComponentInChildren<TextMeshProUGUI>().color = indcompColor;
                indexes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = indcompColor;

                int currentValue = int.Parse(cubes[i].GetComponentInChildren<TextMeshProUGUI>().text);
                int previousValue = int.Parse(cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().text);

                if (currentValue < previousValue)
                {
                    cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;
                    cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

                    Vector3 tempPosition = cubes[i].transform.position;
                    Vector3 newPosition = cubes[i - 1].transform.position;
                    newPosition.y += 1f;

                    while (cubes[i].transform.position != newPosition)
                    {
                        cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, newPosition, Time.deltaTime * swapSpeed);
                        cubes[i - 1].transform.position = Vector3.MoveTowards(cubes[i - 1].transform.position, tempPosition, Time.deltaTime * swapSpeed);
                        yield return null;
                    }

                    GameObject tempCube = cubes[i];
                    cubes[i] = cubes[i - 1];
                    cubes[i - 1] = tempCube;

                    swapped = true;
                    cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                    cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                }

                indexes[i].GetComponentInChildren<TextMeshProUGUI>().color = indexColor;
                indexes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = indexColor;

                if (paused)
                {
                    yield return new WaitWhile(() => paused == true);
                }

                yield return new WaitForSeconds(0.5f);
            }

            cubes[n - iteration].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        }

        cubes[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        sortingInProgress = false;
        iterationText.text = "Sorted";
    }

    public void PauseSorting()
    {
        paused = true;
    }

    public void ResumeSorting()
    {
        paused = false;
    }

    public void ReplaySorting()
    {
        StopAllCoroutines();
        sortingInProgress = false;
        paused = false;
        //GenerateCubes();
    }
}
*/

using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine;


using UnityEngine.UI;
using System.Collections;


public class CubeGenerator : MonoBehaviour
{
    public ARPlaneManager arPlaneManager; // Reference to the ARPlaneManager
    public GameObject cubePrefab;
    public GameObject indexPrefab;
    public TMP_InputField userInputField;
    public Button submitButton;
    public float spacing = 1f;
    public Color textColor = Color.white;
    public Color indexColor = Color.black;
    public Color comparisonColor = Color.yellow;
    public Color indcompColor = Color.red;
    public Color sortedColor = Color.green;
    public float sortingDelay = 1f;
    public float swapSpeed = 12f;
    public GameObject bubbleInputCanvas; // Reference to the BubbleInputCanvas
    public Camera mainCamera;
    public GameObject proAnchor;

private ARPlane trackPlane;
    public TextMeshProUGUI iterationText;
    public TextMeshProUGUI actiontext;
    private GameObject[] cubes;
    private GameObject[] indexes;
    public Canvas exitCanvas;
    public Canvas infoCanvas;
    private bool sortingInProgress = false;
    private bool paused = false;

    private void Start()
    {
        //submitButton.onClick.AddListener(OnSubmitButtonClick);
    }
public void GenerateRandomCubes()
    {
        int cubeCount = Random.Range(5, 10); // Generates a random number between 5 and 10
        string[] randomNumbers = new string[cubeCount];

        for (int i = 0; i < cubeCount; i++)
        {
            randomNumbers[i] = Random.Range(-50, 100).ToString(); // Generates a random number between -100 and 100
        }

        userInputField.text = string.Join(",", randomNumbers); // Sets the input field text to the generated random numbers
        OnSubmitButtonClick(); // Calls the existing method to generate and sort the cubes
    }
    public void OnSubmitButtonClick()
    {
        actiontext.text="";
iterationText.text="";
        bubbleInputCanvas.SetActive(false);

        // Start a coroutine to wait for plane detection
        StartCoroutine(WaitForPlaneDetection());
    }

    private IEnumerator WaitForPlaneDetection()
    {
        actiontext.text = "Don't move the phone.Waiting for plane detection";
        float elapsedTime = 0f;
        float maxWaitTime = 60f; // Maximum wait time in seconds (1 minute)

        while (elapsedTime < maxWaitTime)
        {
            // Check if any planes are detected
            foreach (var trackable in arPlaneManager.trackables)
            {
                if (trackable is ARPlane arPlane)
                {
                    actiontext.text = "plane detected";
                    yield return new WaitForSeconds(10f);
                    // Plane detected, generate cubes on this plane
                    trackPlane = arPlane;
                    GenerateCubesOnPlane(arPlane);
                    yield break; // Exit the coroutine
                }
            }

            // No planes detected yet, wait for a short duration and check again
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        // No plane detected within the time limit, display error message
        Debug.LogError("No AR planes detected within the time limit.");
        actiontext.text = "No AR plane detected within 1 minute.";
    }

    public void GenerateCubesOnPlane(ARPlane plane)
    {
        if (sortingInProgress)
        {
            return;
        }
        DestroyPreviousCubesAndIndices();
        sortingInProgress = true;

        // Hide the BubbleInputCanvas

        // Destroy previous cubes and indexes

        string userInput = userInputField.text;

        string[] numbers = userInput.Split(',');

        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = 0f;
        Vector3 iterationTextPosition = new Vector3(startX - 0.3f, 0f, 0f);
        Vector3 planePosition = plane.transform.position;
        float currentX = startX;
        
        cubes = new GameObject[numbers.Length];
        indexes = new GameObject[numbers.Length];

        // Create a parent object for cubes
        GameObject proAnchor = new GameObject("proAnchor");

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y + 0.5f, planePosition.z + 1f);
             Vector3 indexPosition = new Vector3(planePosition.x + currentX, planePosition.y+0.2f, planePosition.z + 1f);

            // Adjust position relative to the plane
            // Adjust position relative to the plane
            //actiontext.text = "plane" + " " + planePosition + " " + "cube" + " " + cubePosition;

            // Instantiate cube as a child of proAnchor
            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity, proAnchor.transform);
            GameObject index = Instantiate(indexPrefab, indexPosition, Quaternion.identity);

            currentX += spacing * 0.05f;

            cubes[i] = cube;
            indexes[i] = index;

            // Set up cube and index UI
            SetupCubeAndIndexUI(cube, index, numbers[i], i);
        }
        
        movePPRCanvas(exitCanvas,cubes[0].transform.position);
        int n=cubes.Length;
        moveBackCanvas(infoCanvas,cubes[n/2].transform.position);
        // Start sorting coroutine
        StartCoroutine(BubbleSortCoroutine());
    }

    /*
    public void GenerateCubesOnPlane(ARPlane plane)
    {
        
        if (sortingInProgress)
        {
            return;
        }

        sortingInProgress = true;

        // Hide the BubbleInputCanvas

        // Destroy previous cubes and indexes

        string userInput = userInputField.text;
        string[] numbers = userInput.Split(',');

        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = 0f;
        Vector3 iterationTextPosition = new Vector3(startX - 0.3f, 0f, 0f);
        Vector3 planePosition = plane.transform.position;
        float currentX = startX;

        cubes = new GameObject[numbers.Length];
        indexes = new GameObject[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y+0.5f, planePosition.z+1f);

            // Adjust position relative to the plane
            Vector3 indexPosition = plane.transform.position + new Vector3(currentX, -100f, 0f); // Adjust position relative to the plane
            infotext.text = "plane"+" "+planePosition+" "+"cube"+" "+cubePosition ;
            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            GameObject index = Instantiate(indexPrefab, indexPosition, Quaternion.identity);

            currentX += spacing*0.1f;

            cubes[i] = cube;
            indexes[i] = index;

            // Set up cube and index UI
            SetupCubeAndIndexUI(cube, index, numbers[i], i);

        }

        // Start sorting coroutine
        StartCoroutine(BubbleSortCoroutine());
    }
    */
    private void SetupCubeAndIndexUI(GameObject cube, GameObject index, string number, int indexNumber)
    {
        Canvas canvas = cube.GetComponentInChildren<Canvas>();
        Canvas indexCanvas = index.GetComponentInChildren<Canvas>();

        if (canvas != null && indexCanvas != null)
        {
            TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI indexTextMesh = indexCanvas.GetComponentInChildren<TextMeshProUGUI>();

            if (textMesh != null && indexTextMesh != null)
            {
                textMesh.text = number;
                textMesh.color = textColor;
                textMesh.alignment = TextAlignmentOptions.Center;

                indexTextMesh.text = indexNumber.ToString();
                indexTextMesh.color = indexColor;
                indexTextMesh.alignment = TextAlignmentOptions.Center;

                float cubeSize = 24.2f;
                float fontSizeMultiplier = 4f;
                textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
                indexTextMesh.fontSize = Mathf.RoundToInt(cubeSize * 10f);
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube or index prefab.");
            }
        }
        else
        {
            Debug.LogError("Canvas component not found in the children of the cube or index prefab.");
        }
    }

    private IEnumerator BubbleSortCoroutine()
{
    yield return new WaitForSeconds(sortingDelay);

    int n = cubes.Length;
    bool swapped;

    for (int i = 0; i < n - 1; i++)
    {
        iterationText.text = "Iteration: " + (i + 1);
        actiontext.text = "Sorting iteration " + (i + 1);

        swapped = false;
        for (int j = 0; j < n - i - 1; j++)
        {
            // Change color of cubes being compared
            cubes[j].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;
            cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

            // Compare adjacent cubes
            int currentValue = int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);
            int nextValue = int.Parse(cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().text);

            actiontext.text = "Comparing " + currentValue + " and " + nextValue;

            // Display the result of the comparison
            if (currentValue < nextValue)
            {
                actiontext.text += " : " + currentValue + " < " + nextValue;
            }
            else if (currentValue > nextValue)
            {
                actiontext.text += " : " + currentValue + " > " + nextValue;
            }
            else
            {
                actiontext.text += " : " + currentValue + " = " + nextValue;
            }

            yield return new WaitForSeconds(2f); // Delay after comparing

            if (currentValue > nextValue)
            {
                // Lift and swap cubes
                Vector3 tempPosition = cubes[j].transform.position;
                Vector3 newPosition = cubes[j + 1].transform.position;

                

                while (cubes[j].transform.position != newPosition)
                {
                    cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, newPosition, Time.deltaTime * swapSpeed);
                    cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, tempPosition, Time.deltaTime * swapSpeed);

                    // Update action text to show swapping
                    actiontext.text = "Swapping " + currentValue + " and " + nextValue;

                    yield return null;
                }

                // Swap cube references
                GameObject tempCube = cubes[j];
                cubes[j] = cubes[j + 1];
                cubes[j + 1] = tempCube;

                swapped = true;
            }
            else
            {
                actiontext.text = "No Swapping";
            }

            // Reset color after comparison
            cubes[j].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
            cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().color = textColor;

            if (paused)
            {
                yield return new WaitWhile(() => paused == true); // Pause the sorting process
            }

            yield return new WaitForSeconds(0.5f); // Adjust the delay as needed for visualization
        }

        // Display the sorted element and change its color to green
        cubes[n - i - 1].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        actiontext.text = "Element " + cubes[n - i - 1].GetComponentInChildren<TextMeshProUGUI>().text + " is sorted";

       // if (!swapped)
       // {
       //     break; // If no swaps were made, the array is already sorted
       // }
    }

    // Change the color of the first element to green after sorting is completed
    cubes[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
    actiontext.text = "Element " + cubes[0].GetComponentInChildren<TextMeshProUGUI>().text + " is sorted";

    actiontext.text = "Sorting Completed";

    sortingInProgress = false; // Sorting is complete
}
    

    public void PauseSorting()
    {
        paused = true;
    }

    public void ResumeSorting()
    {
        paused = false;
    }

    public void ReplaySorting()
    {
        StopAllCoroutines();
        sortingInProgress = false;
        paused = false;
        GenerateCubesOnPlane(trackPlane);
    }
    public void DestroyPreviousCubesAndIndices()
{
    if (cubes != null)
    {
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
    }

    if (indexes != null)
    {
        foreach (GameObject index in indexes)
        {
            Destroy(index);
        }
    }

    // Reset cube and index arrays
    cubes = null;
    indexes = null;
    sortingInProgress = false;
    actiontext.text="";
iterationText.text="";
}
private void movePPRCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetX = -0.5f; 
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    if (canvasRect != null)
    {
        canvasRect.anchoredPosition3D = position + new Vector3(offsetX, 0f, 0f);
    }
    else
    {
        Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
    }
}
private void moveBackCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetY = 0.5f; 
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    if (canvasRect != null)
    {
        canvasRect.anchoredPosition3D = position + new Vector3(0f, offsetY, 0f);
    }
    else
    {
        Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
    }
}
public void inputFieldClear(TMP_InputField field){
    field.text="";
}


}
