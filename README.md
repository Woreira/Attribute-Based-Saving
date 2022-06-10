# Atribute-Based-Saving

### Usage:  
1. add `[Track]` to your class  
2. add `[Save]` to your field  
3. save (if the tracking cache is not innitialized, it will be initialized automatically)  
4. load (if the tracking cache is not innitialized, it will be initialized automatically)  
> be aware:  
> - unity classes are not serializable, so [Save] will not work on them.  
> - you may need to use SetupSettersAndGetters() manually to properly save recently instantiated objects.  

### Example:  
    //Example.cs
    [Track]
    public class Example : MonoBehaviour{
        [Save]public int x;
        [Save]public float y;
        [Save]public bool z;
    }
