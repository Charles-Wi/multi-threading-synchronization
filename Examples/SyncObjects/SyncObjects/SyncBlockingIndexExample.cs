using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncObjects
{
    internal class SyncBlockingIndexExample
    {
        public static void SyncBlockIndexExample()
        {
            Person person = new() { Id = 3 };
            
            lock (person)
            {
                //Some code here
                var a = 5;
                person.Id = 2;
            }

        }


    }

    public class Person
    {
        public int Id { get; set; } 
    }


}
