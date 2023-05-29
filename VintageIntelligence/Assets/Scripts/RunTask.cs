using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTask : MonoBehaviour
{
    // Start is called before the first frame update
    public async void RunTaskPLS()
    {
        await TaskTest.TestingTask();
    }
}
