using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains information about each tag. 
/// name, color, its parent, if it has a closing or not.
/// </summary>
[Serializable]
public class TagDataModel
{
    public string name;

    private static readonly Dictionary<string, Color> ColorMap = new Dictionary<string, Color>
    {
        #region UFO
        {"ship", Color.green},
        {"alien", Color.magenta},
        {"dome", Color.white},
        #endregion
        
        #region Head
        {"head", Color.green},
        {"eyes", Color.magenta},
        {"nose", Color.magenta},
        {"mouth", Color.magenta},
        {"neck", Color.magenta},
        {"hair", Color.magenta},
        #endregion

        #region Body
        
        {"body", Color.red},

        #region UpperBody
        {"ubody", Color.yellow},
        {"arms", Color.cyan},
        {"hands", Color.cyan},
        {"shirt", Color.cyan},
        #endregion

        #region LowerBody
        {"lbody", new Color(1f,0.647f,0f)},
        {"legs", Color.blue},
        {"pants", Color.blue},
        {"shoes", Color.blue},
        #endregion
        
        #endregion
    };
    
    public static readonly Dictionary<string, TagProp> TagMap = new Dictionary<string, TagProp>
    {
        {"tag", new TagProp{Color = Color.white}},
        
        {"ship", new TagProp{Color = ColorMap["ship"], HasClosing = true}},
        {"/ship", new TagProp{Color = ColorMap["ship"]}},
        
        {"alien", new TagProp{Color = ColorMap["alien"], IsLeaf = true, ParentTag = "ship"}},
        {"dome", new TagProp{Color = ColorMap["dome"], IsLeaf = true, ParentTag = "ship"}},
        
        {"humanoid", new TagProp{Color = Color.white}},
        
        {"head", new TagProp{Color = ColorMap["head"], HasClosing = true}},
        {"/head", new TagProp{Color = ColorMap["head"]}},
        
        {"eyes", new TagProp{Color = ColorMap["eyes"], IsLeaf = true, ParentTag = "head"}},
        {"nose", new TagProp{Color = ColorMap["nose"], IsLeaf = true, ParentTag = "head"}},
        {"mouth", new TagProp{Color = ColorMap["mouth"], IsLeaf = true, ParentTag = "head"}},
        {"neck", new TagProp{Color = ColorMap["neck"], IsLeaf = true, ParentTag = "head"}},
        {"hair", new TagProp{Color = ColorMap["hair"], IsLeaf = true, ParentTag = "head"}},
        
        {"body", new TagProp{Color = ColorMap["body"], HasClosing = true}},
        {"/body", new TagProp{Color = ColorMap["body"]}},
        
        {"ubody", new TagProp{Color = ColorMap["ubody"], HasClosing = true, ParentTag = "body"}},
        {"/ubody", new TagProp{Color = ColorMap["ubody"]}},
        
        {"arms", new TagProp{Color = ColorMap["arms"], IsLeaf = true, ParentTag = "ubody"}},
        {"hands", new TagProp{Color = ColorMap["hands"], IsLeaf = true, ParentTag = "ubody"}},
        {"shirt", new TagProp{Color = ColorMap["shirt"], IsLeaf = true, ParentTag = "ubody"}},
        
        {"lbody", new TagProp{Color = ColorMap["lbody"], HasClosing = true, ParentTag = "body"}},
        {"/lbody", new TagProp{Color = ColorMap["lbody"]}},
        
        {"legs", new TagProp{Color = ColorMap["legs"], IsLeaf = true, ParentTag = "lbody"}},
        {"pants", new TagProp{Color = ColorMap["pants"], IsLeaf = true, ParentTag = "lbody"}},
        {"shoes", new TagProp{Color = ColorMap["shoes"], IsLeaf = true, ParentTag = "lbody"}}
    };

    public class TagProp
    {
        public Color Color;
        public bool HasClosing;
        public bool IsLeaf;
        public string ParentTag = string.Empty;
        public float Offset;
    }
}
