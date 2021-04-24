using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace malvra
{
    /// <summary>
    /// The purpose of this class is to create the mesh to be used in "Siembra en cama".
    /// </summary>
    public class MeshFactory : MonoBehaviour
    {
        private GameObject pinA;
        private GameObject pinB;
        private GameObject mesh;
        private BoundingBox meshBoundingBox;
        private ManipulationHandler meshManipHandler;

        /* Radius references: 
         * http://hydra.nat.uni-magdeburg.de/packing/csq/csq.html#overview
         */
        private Vector3 lastPinAPos;
        private Vector3 lastPinBPos;
        private float currentPlantRadius;

        public GameObject wallPrefab; // Used to represent the mesh's perimeter.
        public GameObject spotPrefab; // Used to represent the planting spots.
        public GameObject pinPrefab; 
        public GameObject flowerButtonPrefab;
        public GameObject commandPanel;

        async void Start()
        {
            List<Flower> flowers = await Flowers.GetFlowers();
            CreateFlowerButtons(flowers);
            this.pinA = GameObject.Find("/SceneContent/Pins/PinA");
            this.pinB = GameObject.Find("/SceneContent/Pins/PinB");
            lastPinAPos = this.pinA.transform.position;
            lastPinBPos = this.pinB.transform.position;
        }

        private void CreateFlowerButtons(List<Flower> flowers)
        {
            GridObjectCollection buttonCollection = commandPanel.GetComponentInChildren<GridObjectCollection>();
            ScrollingObjectCollection buttonCollectionScroll = commandPanel.GetComponentInChildren<ScrollingObjectCollection>();
            float n = 0.0f;
            foreach(Flower flower in flowers)
            {
                GameObject button = GameObject.Instantiate<GameObject>(flowerButtonPrefab, buttonCollection.transform.position, Quaternion.identity);
                button.transform.parent = buttonCollection.transform;
                FlowerProperties buttonFlowerProperties = button.GetComponentInChildren<FlowerProperties>();
                buttonFlowerProperties.Name = flower.Name;
                buttonFlowerProperties.PlantingRadius = flower.PlantingRadius;
                TextMeshPro[] buttonText = button.GetComponentsInChildren<TextMeshPro>();
                foreach(TextMeshPro text in buttonText)
                    text.text = flower.Name;
                Interactable buttonInteractable = button.GetComponentInChildren<Interactable>();
                buttonInteractable.OnClick.AddListener(() => SelectRadius(button));
                n += 0.05f;
            }
            buttonCollection.UpdateCollection();
            buttonCollectionScroll.UpdateContent();
            buttonCollectionScroll.gameObject.SetActive(false);
        }

        public void SelectRadius(GameObject button)
        {
            FlowerProperties buttonFlowerProperties = button.GetComponentInChildren<FlowerProperties>();
            this.currentPlantRadius = buttonFlowerProperties.PlantingRadius;
            ScrollingObjectCollection buttonCollectionScroll = commandPanel.GetComponentInChildren<ScrollingObjectCollection>();
            buttonCollectionScroll.gameObject.SetActive(false);
        }

        public void TogglePlantsMenu()
        {
            ScrollingObjectCollection buttonCollectionScroll = commandPanel.GetComponentInChildren(typeof(ScrollingObjectCollection), true) as ScrollingObjectCollection;
            buttonCollectionScroll.gameObject.SetActive(!buttonCollectionScroll.gameObject.activeSelf);
        }

        public void BuildMesh()
        {
            if (this.currentPlantRadius == 0.0f) return;
            if (ThereIsAMesh()) Reset();

            Vector3 pinApos = this.pinA.transform.position;
            Vector3 pinBpos = this.pinB.transform.position;

            PutPinsAtTheSameHeight(ref pinApos, ref pinBpos);

            Vector3[] cornerPositions = DefineCorners(pinApos, pinBpos);

            Vector3 topLeftCornerPosition = cornerPositions[0];
            Vector3 topRightCornerPosition = cornerPositions[1];
            Vector3 bottomLeftCornerPosition = cornerPositions[2];
            Vector3 bottomRightCornerPosition = cornerPositions[3];

            GameObject topLeftCorner = Instantiate<GameObject>(wallPrefab, topLeftCornerPosition, Quaternion.identity);
            GameObject TopRightCorner = Instantiate<GameObject>(wallPrefab, topRightCornerPosition, Quaternion.identity);
            GameObject BottomLeftCorner = Instantiate<GameObject>(wallPrefab, bottomLeftCornerPosition, Quaternion.identity);
            GameObject BottomRightCorner = Instantiate<GameObject>(wallPrefab, bottomRightCornerPosition, Quaternion.identity);

            AssembleMeshFromCorners(topLeftCorner, TopRightCorner, BottomLeftCorner, BottomRightCorner);

            ConfigureMeshBoundingBox();
            ConfigureMeshManipulationHandler();

            CleanUp(pinApos, pinBpos);
        }

        public void Reset()
        {
            if (ThereIsAMesh()) 
            {
                DeleteMeshAndRecreatePins();
            }
            else
            {
                MovePinsToOriginalPosition();
            }
        }

        private bool ThereIsAMesh()
        {
            return this.pinA == null || this.pinB == null;
        }

        private void DeleteMeshAndRecreatePins()
        {
            GameObject.Destroy(mesh);
            this.pinA = Instantiate<GameObject>(pinPrefab, lastPinAPos, Quaternion.identity);
            this.pinA.name = "PinA";
            this.pinB = Instantiate<GameObject>(pinPrefab, lastPinBPos, Quaternion.identity);
            this.pinB.name = "PinB";
        }

        private void MovePinsToOriginalPosition()
        {
            this.pinA.transform.position = lastPinAPos;
            this.pinB.transform.position = lastPinBPos;
        }

        private void CleanUp(Vector3 pinApos, Vector3 pinBpos)
        {
            // Save the pins' last known locations in case the user wants to reset
            this.lastPinAPos = pinApos;
            this.lastPinBPos = pinBpos;

            GameObject.Destroy(pinA);
            GameObject.Destroy(pinB);
        }

        private void ConfigureMeshManipulationHandler()
        {
            this.meshManipHandler = this.mesh.AddComponent<ManipulationHandler>();
            this.meshManipHandler.OneHandRotationModeFar = ManipulationHandler.RotateInOneHandType.MaintainOriginalRotation;
            this.meshManipHandler.OneHandRotationModeNear = ManipulationHandler.RotateInOneHandType.MaintainOriginalRotation;
        }

        private void ConfigureMeshBoundingBox()
        {
            this.meshBoundingBox = this.mesh.AddComponent<BoundingBox>();
            this.meshBoundingBox.ShowRotationHandleForX = false;
            this.meshBoundingBox.ShowRotationHandleForZ = false;
            this.meshBoundingBox.ShowScaleHandles = false;
        }

        private void PutPinsAtTheSameHeight(ref Vector3 pinApos, ref Vector3 pinBpos)
        {
            if (pinApos.y < pinBpos.y)
            {
                this.pinB.transform.position = new Vector3(pinBpos.x, pinApos.y, pinBpos.z);
                pinBpos = this.pinB.transform.position;
            }
            else
            {
                this.pinA.transform.position = new Vector3(pinApos.x, pinBpos.y, pinApos.z);
                pinApos = this.pinA.transform.position;
            }
        }

        private void AssembleMeshFromCorners(GameObject topLeft, GameObject topRight, GameObject bottomLeft, GameObject bottomRight)
        {
            // Create empty object to hold all the components of the mesh together
            this.mesh = new GameObject();
            this.mesh.name = "Mesh";

            BuildPerimeter(topLeft, topRight, bottomLeft, bottomRight).transform.parent = this.mesh.transform;
            DistributeSpots(topLeft, bottomRight).transform.parent = this.mesh.transform;
        }

        private GameObject DistributeSpots(GameObject topLeft, GameObject bottomRight)
        {
            GameObject spots = new GameObject();
            spots.name = "Spots";

            float x0 = topLeft.transform.position.x;
            float z0 = topLeft.transform.position.z;

            float x1 = bottomRight.transform.position.x;
            float z1 = bottomRight.transform.position.z;

            float diameter = currentPlantRadius * 2;
            int row = 0;
            int total = 0;
            float rowSeparationDistance = diameter - (currentPlantRadius / 4.0f);
            float zstart = z0 + currentPlantRadius;

            for (float z = zstart; z < z1; z += rowSeparationDistance)
            {
                float x = x0 + currentPlantRadius;
                x += (row % 2 == 0) || (row == 0) ? 0 : currentPlantRadius;
                while (x < x1)
                {
                    GameObject spot = Instantiate<GameObject>(spotPrefab, spots.transform.position, Quaternion.identity);
                    Vector3 spotpos = spot.transform.position;
                    spotpos.x = x;
                    spotpos.z = z;
                    spotpos.y = topLeft.transform.position.y + topLeft.transform.localScale.y / 2; /* put spot above the mesh's perimeter fence */
                    spot.transform.position = spotpos;
                    spot.transform.parent = spots.transform;

                    x += diameter;
                    total++;
                }
                row += 1;
            }
            return spots;
        }

        private GameObject BuildPerimeter(GameObject topLeft, GameObject topRight, GameObject bottomLeft, GameObject bottomRight)
        {
            GameObject topSide = BuildSide(topLeft, topRight);
            GameObject botSide = BuildSide(bottomLeft, bottomRight);
            GameObject lefSide = BuildSide(topLeft, bottomLeft);
            GameObject rigSide = BuildSide(topRight, bottomRight);

            // Create empty object to hold all the components of the perimeter together
            GameObject perimeter = new GameObject();
            perimeter.name = "Perimeter";

            // Bind corners to the perimeter object
            topLeft.transform.parent = perimeter.transform;
            topRight.transform.parent = perimeter.transform;
            bottomLeft.transform.parent = perimeter.transform;
            bottomRight.transform.parent = perimeter.transform;

            // Bind sides to the perimeter object
            topSide.transform.parent = perimeter.transform;
            botSide.transform.parent = perimeter.transform;
            lefSide.transform.parent = perimeter.transform;
            rigSide.transform.parent = perimeter.transform;

            return perimeter;
        }

        private GameObject BuildSide(GameObject cornerA, GameObject cornerB)
        {
            GameObject side = Instantiate<GameObject>(wallPrefab, cornerA.transform.position, Quaternion.identity);
            side.name = "Side";

            cornerA.transform.LookAt(cornerB.transform.position);
            cornerB.transform.LookAt(cornerA.transform.position);

            float distance = Vector3.Distance(cornerA.transform.position, cornerB.transform.position);

            side.transform.position = cornerA.transform.position + distance / 2 * cornerA.transform.forward;
            side.transform.rotation = cornerA.transform.rotation;
            side.transform.localScale = new Vector3(side.transform.localScale.x, side.transform.localScale.y, distance);
            return side;
        }

        private Vector3[] DefineCorners(Vector3 a, Vector3 b)
        {
            Vector3[] positions = new Vector3[4];

            float ax = a.x;
            float az = a.z;
            float bx = b.x;
            float bz = b.z;

            /* ====================================================================== *
             * 0-top_left, 1-top_right, 2-bottom_left, 3-bottom_right
             * ---------------------------------------------------------------------- * 
             * Taking as a reference the starting point in the game editor,
             * in reality it may not look this way, but it should work anyways. 
             * ====================================================================== */
            if (az < bz && ax > bx) // a-top_right, b-bottom_left
            {
                positions[0] = new Vector3(b.x, a.y, a.z);
                positions[1] = a;
                positions[2] = b;
                positions[3] = new Vector3(a.x, b.y, b.z);
            }
            else if (az < bz && ax < bx) // a-top_left, b-bottom_right
            {
                positions[0] = a;
                positions[1] = new Vector3(b.x, a.y, a.z);
                positions[2] = new Vector3(a.x, b.y, b.z);
                positions[3] = b;
            }
            else if (az > bz && ax < bx) // b-top_right, a-bottom_left
            {
                positions[0] = new Vector3(a.x, b.y, b.z);
                positions[1] = b;
                positions[2] = a;
                positions[3] = new Vector3(b.x, a.y, a.z);
            }
            else // b-top_left, a-bottom_right
            {
                positions[0] = b;
                positions[1] = new Vector3(a.x, b.y, b.z);
                positions[2] = new Vector3(b.x, a.y, a.z);
                positions[3] = a;
            }
            return positions;
        }
    }
}