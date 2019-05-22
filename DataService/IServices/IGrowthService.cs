using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public interface IGrowthService:IBaseService
    {
        ModelJsonRet GetAlbumsInfo(string nQuery, int cQuery, int pageIndex);
        ModelJsonRet GetNoAlbumStus();
        ModelJsonRet AddSingleAlbum(string name, byte type, int classId, long stuId);
        ModelJsonRet AutoAddStuAlbum(int classId);

        ModelJsonRet GetPhotosInfo(long aId, int pageIndex);
        int AddPhotoRecord(long aId, string describe, string fileUrl, string sizeStr);
        bool DelPhotoRecord(long prId, out string imgUrl);
    }
}
