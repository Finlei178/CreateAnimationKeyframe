/// <summary>
/// Transform����keyframe�쐬
/// </summary>
public class PoseSaveAnim
{
    /// <summary>
    /// �|�[�Y��keyframe�ł�����
    /// </summary>
    /// <param name="setNum">Set number.</param>
    /// <param name="pCurveDic">P curve dic.</param>
    /// <param name="targetTrans">Target transform.</param>
    public void _SetKeyClip(int setNum, ref Dictionary<string, AnimationCurve> pCurveDic, ref AnimationClip clip, Transform targetTrans)
    {
        //Clip Setting
        clip = new AnimationClip();
        clip.legacy = true;

        //�I�u�W�F�N�g�����E�i�[
        List<Transform> PartsParent = new List<Transform>();     //�q�B��ChildCenter
        FindChildObj(targetTrans.GetChild(0).GetChild(0), targetTrans.GetComponent<Module>().m_moduleId, ref PartsParent);

        //�i�[�����I�u�W�F�N�g��transform�����擾
        foreach (Transform trans in PartsParent)
        {
            //Center path�擾
            string path = PathName(targetTrans, trans);

            //Rotation
            KeySet(ref clip, pCurveDic, setNum, setNum * 1, trans.GetComponent<PartsControll>().m_Angle.x, path, "m_Angle.x", typeof(PartsControll));
            KeySet(ref clip, pCurveDic, setNum, setNum * 1, trans.GetComponent<PartsControll>().m_Angle.y, path, "m_Angle.y", typeof(PartsControll));
            KeySet(ref clip, pCurveDic, setNum, setNum * 1, trans.GetComponent<PartsControll>().m_Angle.z, path, "m_Angle.z", typeof(PartsControll));

            //Roll
            if (setNum == 0)
            {
                KeySet(ref clip, pCurveDic, setNum, setNum, 0, path, "pose1AddRotate", typeof(PartsControll));
                KeySet(ref clip, pCurveDic, setNum, setNum, 0, path, "pose2AddRotate", typeof(PartsControll));
                KeySet(ref clip, pCurveDic, setNum, setNum, 0, path, "pose3AddRotate", typeof(PartsControll));
            }
            else
            {
                KeySet(ref clip, pCurveDic, setNum, setNum, pCurveDic[path + "_pose1AddRotate"].Evaluate(setNum - 1), path, "pose1AddRotate", typeof(PartsControll));
                KeySet(ref clip, pCurveDic, setNum, setNum, pCurveDic[path + "_pose2AddRotate"].Evaluate(setNum - 1), path, "pose2AddRotate", typeof(PartsControll));
                KeySet(ref clip, pCurveDic, setNum, setNum, pCurveDic[path + "_pose3AddRotate"].Evaluate(setNum - 1), path, "pose3AddRotate", typeof(PartsControll));
            }

            //Enagy
            string partsPath = PathName(targetTrans, trans.GetChild(0).GetChild(0));
            KeySet(ref clip, pCurveDic, setNum, setNum, trans.GetChild(0).GetChild(0).GetComponent<Parts>().EnagyValue, partsPath, "EnagyValue", typeof(Parts));
        }
    }


    /// <summary>
    /// <param name="anim">�A�j���[�V����</param>
    /// <param name="num">key�̗v�f��</param>
    /// <param name="time">�^�C��</param>
    /// <param name="value">�l</param>
    /// <param name="path">�p�X</param>
    /// <param name="property">�v���p�e�B</param>
    /// <param name="parts">�p�[�c�i��]�p�j</param>
    /// </summary>
    //brief : Set KeyFrame to animationClip
    public void KeySet(ref AnimationClip anim, Dictionary<string, AnimationCurve> curveDic, int num, float time, float value, string path, string property, System.Type type)
    {
        Keyframe key = new Keyframe(time, value);
        AnimationCurve curve = new AnimationCurve();

        // Dictionary��key�Ɉ��������邩
        if (curveDic.ContainsKey(path + "_" + property))
        {
            //����ΎQ��
            curve = curveDic[path + "_" + property];
        }
        // �Ȃ���Βǉ�
        else curveDic.Add(path + "_" + property, curve);
        
        //AddKey
        int keyNum = curve.AddKey(key);
        //ReAddKey
        if (keyNum == -1) curve.MoveKey(num, key);

        //Linear�ύX
        if (num > 0)
        {
            Keyframe prev = curve.keys[num - 1];
            Keyframe next = curve.keys[num];
            //KeyFrame�R�s�[
            Keyframe prevKey = new Keyframe(prev.time, prev.value, prev.inTangent, prev.outTangent);
            Keyframe nextKey = new Keyframe(next.time, next.value, next.inTangent, next.outTangent);

            float tang = 0;
            if (path.Contains("eularAngle"))
                _GetLinearTangentForRotation(prevKey, nextKey);
            else
                tang = (nextKey.value - prevKey.value) / (nextKey.time - prevKey.time);
            prevKey.outTangent = tang;
            nextKey.inTangent = tang;

            curve.MoveKey(num - 1, prevKey);
            curve.MoveKey(num, nextKey);
        }
        //Set AnimationCurve to AnimationClip
        anim.SetCurve(path, type, property, curve);
    }

}


