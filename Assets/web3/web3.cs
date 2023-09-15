using System.Collections;
using System.Threading.Tasks;
using Thirdweb;
using UnityEngine;
using UnityEngine.Networking;

public class Web3 : MonoBehaviour
{
    private ThirdwebSDK sdk;
    private string assetBundleUrl;

    async void Start()
    {
        sdk = new ThirdwebSDK("mumbai");
        await LoadNft();
        StartCoroutine(SpawnNft());
    }
	
	async Task<string> LoadNft()
{
    var contract =
        sdk.GetContract("0xC1844aac1F49667E187fd2149E530956dA522a66");
    var nft = await contract.ERC721.Get("0");
    assetBundleUrl = nft.metadata.image;
    return assetBundleUrl;
}

IEnumerator SpawnNft()
{
    // Define the prefab name of the asset you're instantiating here.
    string assetName = "elephant";
    
    // Request the asset bundle from the IPFS URL
    UnityWebRequest www =
        UnityWebRequestAssetBundle.GetAssetBundle(assetBundleUrl);
    yield return www.SendWebRequest();
    
    // Something failed with the request.
    if (www.result != UnityWebRequest.Result.Success)
    {
        Debug.Log("Network error");
        Debug.Log(www.error);
    }
    
    // Successfully downloaded the asset bundle, instantiate the prefab now.
    else
    {
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
        GameObject prefab = bundle.LoadAsset<GameObject>(assetName);
        GameObject instance =
            Instantiate(prefab, new Vector3(0, 3, 5), Quaternion.identity);
            
        // (Optional) - Configure the shader of your NFT as it renders.
        Material material = instance.GetComponent<Renderer>().material;
        material.shader = Shader.Find("Standard");
    }
}
}
