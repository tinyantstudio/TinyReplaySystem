using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TinyReplay
{
    /// <summary>
    /// Replay object base state.
    /// </summary>
    public class TinyReplayObjectState
    {
        // for rotation and position.
        private bool changePos = false;
        private Vector3 mVectorPos;
        private bool changeRot = false;
        private Vector3 mVectorRot;

        // for color data.
        private bool changeColor = false;
        private float r;
        private float g;
        private float b;
        private float a;

        private int mTimePosition = -1;
        private int mEntityIndex = -1;
        public int TimePos
        {
            get { return this.mTimePosition; }
        }

        public int EntityIndex
        {
            get { return this.mEntityIndex; }
        }
        public void ResetState()
        {
            this.changeColor = false;
            this.changePos = false;
            this.changeRot = false;
            this.mTimePosition = -1;
            this.mEntityIndex = -1;
        }

        public static string SaveCurStateProperties(int entityIndex,
            int timePos,
            Transform targetTrs,
            UITexture targetTexture,
            int saveType)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0};{1};", entityIndex, timePos);

            int targetType = 1 << ((int)SaveTargetPropertiesType.Position);
            if ((saveType & targetType) == targetType)
            {
                // save the position.
                sb.AppendFormat("{0},{1},{2}|",
                    targetTrs.localPosition.x,
                    targetTrs.localPosition.y,
                    targetTrs.localPosition.z);
            }
            else
                sb.Append("|");

            targetType = 1 << ((int)SaveTargetPropertiesType.Rotation);
            if ((saveType & targetType) == targetType)
            {
                // save rotation.
                sb.AppendFormat("{0},{1},{2}|",
                    targetTrs.localEulerAngles.x,
                    targetTrs.localEulerAngles.y,
                    targetTrs.localEulerAngles.z);
            }
            else
                sb.Append("|");

            targetType = 1 << ((int)SaveTargetPropertiesType.Color);
            if ((saveType & targetType) == targetType && targetTexture != null)
            {
                // save color value.
                sb.AppendFormat("{0},{1},{2},{3}|",
                    targetTexture.color.r,
                    targetTexture.color.g,
                    targetTexture.color.b,
                    targetTexture.color.a);
            }
            else
                sb.Append("|");
            return sb.ToString();
        }

        public void ParsingProperties(int entityIndex, int timePos, string targetStr)
        {
            // should be optimized for memory GC call.
            this.mTimePosition = timePos;
            this.mEntityIndex = entityIndex;
            
            string[] properties = targetStr.Split('|');
            // position.
            this.ParsingPosition(properties[0]);
            // rotation.
            this.ParsingRotation(properties[1]);
            // color.
            this.ParsingColor(properties[2]);
        }
        
        private void ParsingPosition(string strPosition)
        {
            if (string.IsNullOrEmpty(strPosition))
                return;
            this.changePos = true;
            string[] strPos = strPosition.Split(',');
            this.mVectorPos.x = float.Parse(strPos[0]);
            this.mVectorPos.y = float.Parse(strPos[1]);
            this.mVectorPos.z = float.Parse(strPos[2]);
        }

        private void ParsingRotation(string strRotation)
        {
            if (string.IsNullOrEmpty(strRotation))
                return;
            this.changeRot = true;
            string[] strRot = strRotation.Split(',');
            this.mVectorRot.x = float.Parse(strRot[0]);
            this.mVectorRot.y = float.Parse(strRot[1]);
            this.mVectorRot.z = float.Parse(strRot[2]);
        }

        private void ParsingColor(string strColor)
        {
            if (string.IsNullOrEmpty(strColor))
                return;
            this.changeColor = true;
            string[] color = strColor.Split(',');
            this.r = float.Parse(color[0]);
            this.g = float.Parse(color[1]);
            this.b = float.Parse(color[2]);
            this.a = float.Parse(color[3]);
            // Debug.Log("@color changed.");
        }
        public void SynchronizeProperties(Transform trs)
        {
            if (this.changePos)
                trs.localPosition = this.mVectorPos;
            if (this.changeRot)
                trs.localEulerAngles = this.mVectorRot;
        }

        public void SynchronizeColorProperties(UITexture texture, Color tmpColor)
        {
            if (this.changeColor)
            {
                tmpColor.r = this.r;
                tmpColor.g = this.g;
                tmpColor.b = this.b;
                tmpColor.a = this.a;
                texture.color = tmpColor;
                // Debug.Log("@change target texture color.");
            }
        }
    }
}