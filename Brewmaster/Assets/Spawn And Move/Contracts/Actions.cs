// Generated by dojo-bindgen on Mon, 3 Jun 2024 19:57:17 +0000. Do not modify this file manually.
using System;
using System.Threading.Tasks;
using Dojo;
using Dojo.Starknet;
using UnityEngine;
using dojo_bindings;
using System.Collections.Generic;
using System.Linq;
using Enum = Dojo.Starknet.Enum;

// System definitions for `dojo_examples::actions::actions` contract
public class Actions : MonoBehaviour
{
    // The address of this contract
    public FieldElement contractAddress;


    // Call the `dojo_init` system with the specified Account and calldata
    // Returns the transaction hash. Use `WaitForTransaction` to wait for the transaction to be confirmed.
    public async Task<FieldElement> dojo_init(Account account)
    {
        List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();


        return await account.ExecuteRaw(new dojo.Call[] {
            new dojo.Call{
                to = contractAddress.Inner,
                selector = "dojo_init",
                calldata = calldata.ToArray()
            }
        });
    }



    // Call the `spawn` system with the specified Account and calldata
    // Returns the transaction hash. Use `WaitForTransaction` to wait for the transaction to be confirmed.
    public async Task<FieldElement> spawn(Account account)
    {
        List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();


        return await account.ExecuteRaw(new dojo.Call[] {
            new dojo.Call{
                to = contractAddress.Inner,
                selector = "spawn",
                calldata = calldata.ToArray()
            }
        });
    }



    // Call the `move` system with the specified Account and calldata
    // Returns the transaction hash. Use `WaitForTransaction` to wait for the transaction to be confirmed.
    public async Task<FieldElement> move(Account account, Direction direction)
    {
        List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();
        calldata.Add(new FieldElement(Enum.GetIndex(direction)).Inner);

        return await account.ExecuteRaw(new dojo.Call[] {
            new dojo.Call{
                to = contractAddress.Inner,
                selector = "move",
                calldata = calldata.ToArray()
            }
        });
    }



    // Call the `set_player_config` system with the specified Account and calldata
    // Returns the transaction hash. Use `WaitForTransaction` to wait for the transaction to be confirmed.
    // public async Task<FieldElement> set_player_config(Account account, string name) {
    //     List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();
    //     calldata.AddRange(ByteArray.Serialize(name).Select(f => f.Inner));

    //     return await account.ExecuteRaw(new dojo.Call[] {
    //         new dojo.Call{
    //             to = contractAddress.Inner,
    //             selector = "set_player_config",
    //             calldata = calldata.ToArray()
    //         }
    //     });
    // }

}
