using SAS.Async;
using UnityEngine;
public class TestRoutien : MonoBehaviour
{
    public RoutineManagerBehaviour routineManager;
    // Start is called before the first frame update
    public async void Start()
    {
        Debug.Log(Time.frameCount);
        routineManager.Run(PrintAnyOfTheNumbers());
        Debug.Log(Time.frameCount);
    }

    public async AsyncTask Countdown()
    {
        await Routine.WaitForNextFrame();
        for (var i = 10; i >= 0; --i)
        {
            Debug.Log($"fddfff + {i}");
            await Routine.WaitForSeconds(1);
        }
    }

    public async AsyncTask PrintTheNumber()
    {
        var theNum = await GetTheNumber();
        Debug.Log(theNum);
    }
    public async AsyncTask<int> GetTheNumber()
    {
        await AsyncTask.WaitForSeconds(1);
        return 17;
    }

    public async AsyncTask PrintAllOfTheNumbers()
    {
        //numbers is an int[] containing all of the results in order
        var numbers = await Routine.WhenAll(GetTheFirstNumber(), GetTheSecondNumber(), GetTheThirdNumber());
        foreach (var num in numbers)
        {
            Debug.Log(num);
        }
    }

    public async AsyncTask PrintAnyOfTheNumbers()
    {
        //num is the result of the first routine to finish
        var num = await Routine.WhenAny(GetTheSecondNumber(), GetTheFirstNumber(), GetTheThirdNumber());
        Debug.Log(num);
    }

    public async AsyncTask<int> GetTheFirstNumber()
    {
        await Routine.WaitForSeconds(30);
        return 1;
    }

    public async AsyncTask<int> GetTheSecondNumber()
    {
        await Routine.WaitForSeconds(20);
        return 2;
    }

    public async AsyncTask<int> GetTheThirdNumber()
    {
        await Routine.WaitForSeconds(1);
        return 3;
    }


}
