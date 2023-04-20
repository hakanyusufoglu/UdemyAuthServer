using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.Configuration
{
   public class Client //projede ki authservera istek yapıcak uygulamaları temsil ediyor. mobilde olabilir web uygulamada olabilir.
    {
        public string Id { get; set; } //bu ifadeler OAuth2.0'dan gelen isimlendirmeler
        public string Secret { get; set; }
        //mesela www.myap1.com, www.myapi2.com bu ikisi varsa client isteklerinde dönen tokenda bu iki app erişebilir ama myapi3.com erişemeyecek çünkü yok.
        public List<String> Audiences { get; set; } //iç mekanizmada benim apilerimden hangileri erişecek onu belirtiyoruz. 
    }
}
