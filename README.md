# cloth-craft-project-with-tuning-editor

Fine feeling & fine adjustment of a mobile game project.

## Gameplay

Watch [Video](https://youtu.be/FWWaN24vIyg)  - Download [APK](https://drive.google.com/file/d/19ERU-i01E-EoJ4N6BzgAcXUlZwcJOolZ/view?usp=sharing)
<div align="left">
      <a href="https://youtu.be/FWWaN24vIyg">
     <img 
src="https://img.youtube.com/vi/FWWaN24vIyg/0.jpg" 
      alt="Everything Is AWESOME" 
      style="width:25%;">
      </a>
    </div>



## Tuning Editor Feature

[Video](https://youtube.com/shorts/AHbjWU1MHbk?feature=share)

<div align="left">
      <a href="https://youtube.com/shorts/AHbjWU1MHbk?feature=share">
     <img 
src="https://img.youtube.com/vi/YJn0MqMc-u0/0.jpg" 
      alt="Everything Is AWESOME" 
      style="width:40%;">
      </a>
    </div>


## Scripting Samples

### Extendable pooling sample using scriptables

<img width="344" alt="Screenshot 2023-11-24 at 13 34 39" src="https://github.com/bk-kurt/cloth-craft-project-with-tuning-editor/assets/128593759/7deba5e4-6cef-4411-ac55-880db35bda5a">


<img width="344" alt="Screenshot 2023-11-24 at 13 30 28" src="https://github.com/bk-kurt/cloth-craft-project-with-tuning-editor/assets/128593759/a20eb8d7-bce5-42c8-aa36-58817b61ce18">


 ### Monosingleton as creational practicality
 ```C#
public class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T>
{
    private static volatile T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType(typeof(T)) as T;
            }
            return instance;
        }
    }
}
```

 ### Factory use case
```C#
private IEnumerator SpawnExecutableForIdleState()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // Only try to spawn if there's an available slot.
            if (IsSlotAvailable())
            {
                ExecutableObject newObject = factory.CreateObject();
                TrySpawn(newObject);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
```


## License

MIT License
