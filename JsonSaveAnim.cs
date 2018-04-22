
//------------------------------
// Json‚©‚çAnimation‚ğì¬
//------------------------------
public class JsonSaveAnim
{
    /// <summary>
    /// moduledateJson‚©‚çAnimationCurveì¬
    /// </summary>
    /// <param name="getMotionDate"></param>
    /// <returns></returns>
    public AnimationCurve CreateCurve(Json getMotionDate)
    {
        //AnimationCurve
        AnimationCurve  _curve = new AnimationCurve();
        Keyframe[]      _keyframe = new Keyframe[getMotionDate._module_registerMotionList_poseData.Count];

        //Keyframe‚Ìæ“¾
        for (int i = 0; i < getMotionDate._module_registerMotionList_poseData.Count; i++)
        {
            //Keyframe
            _keyframe[i] = new Keyframe(
              getMotionDate._module_registerMotionList_poseData[i].module_registerdMotionList_motionData_poseData_poseKeyTime,
              getMotionDate._module_registerMotionList_poseData[i].module_registerdMotionList_motionData_poseData_poseKeyValue
              );

            //KeyFrame‚Ì’Ç‰Á
            _curve.AddKey(_keyframe[i]);
        }
        return _curve;
    }
    


    /// <summary>
    /// animtionCurve‚©‚çClipì¬ˆ—
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="animDic"></param>
    /// <returns></returns>
    public IEnumerator moduleAddClip(Animation animation, Dictionary<string, AnimationCurve> animDic)
    {
        //AnimationClip
        AnimationClip anim = new AnimationClip();
        anim.legacy = true;
        anim.name = "AnimClip";

        foreach (var dic in animDic)
        {
            string _path = dic.Key.Split('_')[0];
            string _property = dic.Key.Replace(_path + "_", "");
            Type type = typeof(Transform);

            if (_property.IndexOf("AddRotate") >= 0)
            {
                type = typeof(PartsControll);
                anim.SetCurve(_path, type, _property, dic.Value);
            }
            if (_property.IndexOf("m_Angle") >= 0)
            {
                type = typeof(PartsControll);
                anim.SetCurve(_path, type, _property, dic.Value);
            }
            if (_property.IndexOf("EnagyValue") >= 0)
            {
                type = typeof(Parts);
                anim.SetCurve(_path, type, _property, dic.Value);
            }
        }
        animation.AddClip(anim, anim.name);
        animation.GetComponent<Animation>().clip = anim;
    }
}
