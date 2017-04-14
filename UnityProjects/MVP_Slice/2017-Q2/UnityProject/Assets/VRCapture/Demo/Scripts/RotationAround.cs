using UnityEngine;

namespace VRCapture.Demo {

    public class RotationAround : MonoBehaviour {

        public GameObject centerPoint;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            this.transform.RotateAround(centerPoint.transform.position, Vector3.up, 100 * Time.deltaTime);
        }
    }
}