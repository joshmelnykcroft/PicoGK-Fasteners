﻿try
{
    PicoGK.Library.Go(0.1f, CarinaLabs.FastenerExample.Task);
}
catch (Exception e)
{
    Console.WriteLine("/n Yeah, nope./n");
    Console.WriteLine(e.ToString());
}
