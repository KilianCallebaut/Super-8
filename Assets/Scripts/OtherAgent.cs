using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class for info agents can save from other agents
 *
 */

public class OtherAgent {

    public Vector3 Position { get; set; }
    public string Name { get; set; }

    public OtherAgent( string name, Vector3 position)
    {
        this.Position = position;
        this.Name = name;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        OtherAgent other = obj as OtherAgent;
        if (other == null) return false;
        else
        {
            if ( other.Name == this.Name )
            {
                return true;
            }  
        }
        return false;
    }
}
