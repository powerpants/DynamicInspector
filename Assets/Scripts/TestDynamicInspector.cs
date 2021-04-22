using DynamicInspector.Attributes;
using UnityEngine;

[CustomInspectorGUI]
public class TestDynamicInspector : MonoBehaviour
{
    public bool @switch;

    [DynamicHidden("switch", true, false)] 	    public string text1 = "DynamicHidden(field,true,false)";
    [DynamicHidden("switch", true, true)] 	    public string text2 = "DynamicHidden(field,true,true)";
        
    [DynamicHidden("switch", false)] 		    public string text3 = "DynamicHidden(field,false)";
    [DynamicHidden("switch", false, true)] 	    public string text4 = "DynamicHidden(field,false,true)";
        
    [ReadOnly][DynamicHidden("switch", false)] 	public string text5 = "ReadOnly DynamicHidden(field,true,false)";
    [ReadOnly][DynamicHidden("switch", true)] 	public string text6 = "ReadOnly DynamicHidden(field,true,false)";
        
    [DynamicHidden("")] 	                    public string text7 = "DynamicHidden(\"\")";
    [ReadOnly][DynamicHidden("")] 	            public string text8 = "ReadOnly DynamicHidden(\"\")";
        
    [DynamicHidden("", true)] 	                public string text9 = "DynamicHidden(\"\",true)";
    [ReadOnly][DynamicHidden("", true)] 	    public string text10 = "ReadOnly DynamicHidden(\"\",true)";
        
    [ReadOnly] 	                                public string text11 = "ReadOnly";
        
    [DynamicHidden("switch", null)] 	        public string text12 = "ReadOnly DynamicHidden(field,null)";

}
