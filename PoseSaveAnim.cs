/// <summary>
/// Transformからkeyframe作成
/// </summary>
public class PoseSaveAnim
{
    /// <summary>
    /// ポーズのkeyframe打ち込み
    /// </summary>
    /// <param name="setNum">Set number.</param>
    /// <param name="pCurveDic">P curve dic.</param>
    /// <param name="targetTrans">Target transform.</param>
    public void _SetKeyClip(int setNum, ref Dictionary<string, AnimationCurve> pCurveDic, ref AnimationClip clip, Transform targetTrans)
    {
        //Clip Setting
        clip = new AnimationClip();
        clip.legacy = true;

        //オブジェクト検索・格納
        List<Transform> PartsParent = new List<Transform>();     //子達のChildCenter
        FindChildObj(targetTrans.GetChild(0).GetChild(0), targetTrans.GetComponent<Module>().m_moduleId, ref PartsParent);

        //格納したオブジェクトのtransform情報を取得
        foreach (Transform trans in PartsParent)
        {
            //Center path取得
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
    /// <param name="anim">アニメーション</param>
    /// <param name="num">keyの要素数</param>
    /// <param name="time">タイム</param>
    /// <param name="value">値</param>
    /// <param name="path">パス</param>
    /// <param name="property">プロパティ</param>
    /// <param name="parts">パーツ（回転用）</param>
    /// </summary>
    //brief : Set KeyFrame to animationClip
    public void KeySet(ref AnimationClip anim, Dictionary<string, AnimationCurve> curveDic, int num, float time, float value, string path, string property, System.Type type)
    {
        Keyframe key = new Keyframe(time, value);
        AnimationCurve curve = new AnimationCurve();

        // Dictionaryのkeyに引っかかるか
        if (curveDic.ContainsKey(path + "_" + property))
        {
            //あれば参照
            curve = curveDic[path + "_" + property];
        }
        // なければ追加
        else curveDic.Add(path + "_" + property, curve);
        
        //AddKey
        int keyNum = curve.AddKey(key);
        //ReAddKey
        if (keyNum == -1) curve.MoveKey(num, key);

        //Linear変更
        if (num > 0)
        {
            Keyframe prev = curve.keys[num - 1];
            Keyframe next = curve.keys[num];
            //KeyFrameコピー
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


