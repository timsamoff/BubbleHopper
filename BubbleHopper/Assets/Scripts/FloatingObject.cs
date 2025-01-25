using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WaterStylizedShader
{
    [RequireComponent(typeof(Rigidbody))]
    public class FloatingObject : MonoBehaviour
    {
        [SerializeField] private Transform[] floaters;
        [SerializeField] private float underWaterDrag = 3f;
        [SerializeField] private float underWaterAngularDrag = 1f;
        [SerializeField] private float airWaterDrag = 0f;
        [SerializeField] private float airWaterAngularDrag = 0.05f;

        [SerializeField] private float floatingPower = 15f;

        [SerializeField] private float baseWaterHeight = 0f;
        [SerializeField] private float waterHeightVariation = 2f;
        [SerializeField] private float waveSpeed = 1.0f;
        [SerializeField] private float waterHeight;

        Rigidbody rb;
        int floatersUnderwater;
        bool underwater;
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            waterHeight = baseWaterHeight + Mathf.Sin(Time.time * waveSpeed) * (waterHeightVariation / 2f);

            floatersUnderwater = 0;
            for (int i = 0; i < floaters.Length; i++)
            {
                float diff = floaters[i].position.y - waterHeight;

                if (diff < 0)
                {
                    rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(diff), floaters[i].position, ForceMode.Force);
                    floatersUnderwater++;
                    if (!underwater)
                    {
                        underwater = true;
                        SwitchState(true);
                    }
                }
            }


            if (underwater && floatersUnderwater == 0)
            {
                underwater = false;
                SwitchState(false);
            }
        }

        void SwitchState(bool isUnderwater)
        {
            if (isUnderwater)
            {
                rb.linearDamping = underWaterDrag;
                rb.angularDamping = underWaterAngularDrag;
            }
            else
            {
                rb.linearDamping = airWaterDrag;
                rb.angularDamping = airWaterAngularDrag;
            }
        }
    }
}

