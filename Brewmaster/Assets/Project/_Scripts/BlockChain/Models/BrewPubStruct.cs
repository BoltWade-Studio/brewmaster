// Generated by dojo-bindgen on Thu, 5 Sep 2024 16:48:40 +0000. Do not modify this file manually.
using System;
using Dojo;
using Dojo.Starknet;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Enum = Dojo.Starknet.Enum;
using System.Numerics;

// Type definition for `brewmaster::models::brewpub::PubScaleStruct` struct

[Serializable]
public struct PubScaleStruct
{
    public ushort tableIndex;
    public ushort stools;
}

// Type definition for `core::integer::u256` struct
[Serializable]
public struct U256
{
    public BigInteger low;
    public BigInteger high;
}


// Model definition for `brewmaster::models::brewpub::BrewPubStruct` model
public class BrewPubStruct : ModelInstance
{
    [ModelField("player")]
    public FieldElement player;

    [ModelField("scale")]
    public PubScaleStruct[] scale;

    [ModelField("treasury")]
    public U256 treasury;

    [ModelField("points")]
    public U256 points;

    [ModelField("createAt")]
    public ulong createAt;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
