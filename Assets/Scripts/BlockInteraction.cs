using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {

	public GameObject cam;
	Block.BlockType buildtype = Block.BlockType.STONE;
	
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown("1"))
			buildtype = Block.BlockType.WATER;
		if(Input.GetKeyDown("2"))
			buildtype = Block.BlockType.SAND;
		if(Input.GetKeyDown("3"))
			buildtype = Block.BlockType.DIRT;
		if(Input.GetKeyDown("4"))
			buildtype = Block.BlockType.STONE;
		if(Input.GetKeyDown("5"))
			buildtype = Block.BlockType.DIAMOND;
		if(Input.GetKeyDown("6"))
			buildtype = Block.BlockType.REDSTONE;
		if(Input.GetKeyDown("7"))
			buildtype = Block.BlockType.GOLD;

		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            
            //for mouse clicking
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
   			//if ( Physics.Raycast (ray,out hit,10)) 
   			//{
            
   			//for cross hairs
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
   				Chunk hitc;
   				if(!World.chunks.TryGetValue(hit.collider.gameObject.name, out hitc)) return;

   				Vector3 hitBlock;
   				if(Input.GetMouseButtonDown(0))
   				{
   					hitBlock = hit.point - hit.normal/2.0f;
   					
   				}
   				else
   				 	hitBlock = hit.point + hit.normal/2.0f;

   				//int x = (int) (Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
   				//int y = (int) (Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
   				//int z = (int) (Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);
				
				Block b = World.GetWorldBlock(hitBlock);
				//Debug.Log(b.position);
				hitc = b.owner;

				bool update = false;
				if(Input.GetMouseButtonDown(0))
					update = b.HitBlock();
				else
				{
					
					update = b.BuildBlock(buildtype);
				}
				
				if(update)
   				{
   					hitc.changed = true;
	   				List<string> updates = new List<string>();
	   				float thisChunkx = hitc.chunk.transform.position.x;
	   				float thisChunky = hitc.chunk.transform.position.y;
	   				float thisChunkz = hitc.chunk.transform.position.z;

	   				//updates.Add(hit.collider.gameObject.name);

	   				//update neighbours?
	   				if(b.position.x == 0) 
	   					updates.Add(World.BuildChunkName(new Vector3(thisChunkx-World.chunkSize,thisChunky,thisChunkz)));
					if(b.position.x == World.chunkSize - 1) 
						updates.Add(World.BuildChunkName(new Vector3(thisChunkx+World.chunkSize,thisChunky,thisChunkz)));
					if(b.position.y == 0) 
						updates.Add(World.BuildChunkName(new Vector3(thisChunkx,thisChunky-World.chunkSize,thisChunkz)));
					if(b.position.y == World.chunkSize - 1) 
						updates.Add(World.BuildChunkName(new Vector3(thisChunkx,thisChunky+World.chunkSize,thisChunkz)));
					if(b.position.z == 0) 
						updates.Add(World.BuildChunkName(new Vector3(thisChunkx,thisChunky,thisChunkz-World.chunkSize)));
					if(b.position.z == World.chunkSize - 1) 
						updates.Add(World.BuildChunkName(new Vector3(thisChunkx,thisChunky,thisChunkz+World.chunkSize)));

		   			foreach(string cname in updates)
		   			{
		   				Chunk c;
						if(World.chunks.TryGetValue(cname, out c))
						{
							c.Redraw();
				   		}
				   	}
				}
		   	}
   		}
	}
}

