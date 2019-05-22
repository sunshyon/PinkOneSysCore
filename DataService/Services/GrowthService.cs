using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Domain;

namespace DataService
{
    public class GrowthService:BaseService,IGrowthService
    {
        #region 照片相关
        public int AddPhotoRecord(long aId,string describe, string fileUrl,string sizeStr)
        {
            var newObj = new SYS_PhotoRecord
            {
                AlbumId = aId,
                SchoolId = mlUser.School.ID,
                Describe = describe,
                ImgUrl = fileUrl,
                SizeStr = sizeStr,
                CreateTime = DateTime.Now,
            };
            UnitOfWork.Repository<SYS_PhotoRecord>().AddEntity(newObj);
            var isOk= UnitOfWork.CommitAsync().Result;
            if (isOk > 0)
            {
                //更新相册相关信息
                var album = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID&& x.ID == aId).Result.FirstOrDefault();
                if (album != null)
                {
                    var pCount = album.PhotoCount;
                    album.PhotoCount = pCount+1;
                    album.Preview = fileUrl;
                    album.UpdateTime = DateTime.Now;
                    UnitOfWork.Repository<SYS_Album>().UpdateEntity(album);
                   var a= UnitOfWork.CommitAsync().Result;
                }
            }
            return isOk;
        }
        public bool DelPhotoRecord(long prId, out string imgUrl)
        {
            var res = false;
            imgUrl = "";
            var photoR = UnitOfWork.Repository<SYS_PhotoRecord>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.ID == prId).Result.FirstOrDefault();
            if (photoR != null)
            {
                imgUrl = photoR.ImgUrl;
                var album = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.ID == photoR.AlbumId).Result.FirstOrDefault();
                if (album != null)
                {
                    //相册预览图为该图，删除前需要更新相册预览图地址
                    if (photoR.ImgUrl == album.Preview)
                    {
                        var newPhoto = UnitOfWork.Repository<SYS_PhotoRecord>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.AlbumId == album.ID).Result
                            .OrderByDescending(x => x.CreateTime).First();
                        if (newPhoto != null)
                        {
                            album.Preview = newPhoto.ImgUrl;
                        }
                    }
                    UnitOfWork.Repository<SYS_PhotoRecord>().DeleteEntity(photoR);
                    var isOK = UnitOfWork.CommitAsync().Result;
                    if (isOK > 0)
                    {
                        res = true;
                        if (album.PhotoCount > 0)
                            album.PhotoCount -= 1;
                        UnitOfWork.Repository<SYS_Album>().UpdateEntity(album);
                        var isOK2=UnitOfWork.CommitAsync().Result;
                    }
                }
                
            }
            return res;
        }
        
        public ModelJsonRet GetPhotosInfo(long aId,int pageIndex)
        {
            var pageItemCount = 12;
            var album = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.ID == aId).Result.FirstOrDefault();
            if (album != null)
            {
                var totalCount = UnitOfWork.Repository<SYS_PhotoRecord>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.AlbumId == aId).Result.Count();
                if (totalCount != album.PhotoCount)
                {
                    album.PhotoCount = totalCount;
                    UnitOfWork.Repository<SYS_Album>().UpdateEntity(album);
                    var isOK= UnitOfWork.CommitAsync().Result;
                }
                var albumTitle = "相册名：" + album.AlbumName + "&emsp; 照片数：" + album.PhotoCount + "&ensp;张 &emsp; 类型：" + (AlbumType)album.Type +
                    "&emsp;创建日期：" + ((DateTime)album.CreateTime).ToString("yyyy-MM-dd");
                var totalPage = 1;
                if (totalCount > pageItemCount)
                {
                    totalPage = totalCount / pageItemCount + 1;
                }
                var photos = UnitOfWork.Repository<SYS_PhotoRecord>().GetEntitiesForPageOrderByDescAsync(x => x.SchoolId == mlUser.School.ID && x.AlbumId == aId,
                    x=>x.CreateTime, (pageIndex - 1) * pageItemCount, pageItemCount).Result;
                  


                var tipStr = "无照片，请添加";
                var sb = new StringBuilder("<div style='margin-left:50%;'><h4 class='text-danger'>" + tipStr + "</h4></div>");
                if (photos.Count > 0)
                {
                    sb = new StringBuilder();
                    foreach (var item in photos)
                    {
                        var time= ((DateTime)item.CreateTime).ToString("yyyy/MM/dd");
                        sb.Append("<div class='box'><div class='box-body' style='background-image:url("+item.ImgUrl+")'></div>");
                        sb.Append("<div class='box-footer'><b>"+ time + "&ensp;"+item.Describe+ "</b><i class='fa fa-trash' onclick='delPhoto(" + item.ID + ")'></i></div></div>");
                    }
                }
                var json = new
                {
                    totalPage = totalPage,
                    albumTitle = albumTitle,
                    photoStr = sb.ToString()
                };
                mjRet.code = 1;
                mjRet.content = json;
            }
            return mjRet;
        }
        #endregion

        #region 相册相关
        public ModelJsonRet AutoAddStuAlbum(int classId)
        {
            var sql = "select*from SYS_Student where Status=1 and SchoolId=" + mlUser.School.ID + " and ClassId=" + classId + 
                " and ID not in(select s.ID from SYS_Student as s inner join SYS_Album as a on s.ID=a.MasterId where s.Status=1 and s.SchoolId=" + mlUser.School.ID + " and s.ClassId=" + classId+")";
            var noAlbumStus = UnitOfWork.ExecuteSqlQuery<SYS_Student>(sql).ToList();
            if (noAlbumStus.Count > 0)
            {
                foreach(var stu in noAlbumStus)
                {
                    var album = new SYS_Album
                    {
                        SchoolId = mlUser.School.ID,
                        AlbumName = stu.StuName,
                        Type = (byte)AlbumType.个人相册,
                        ClassId = stu.ClassId,
                        MasterId = stu.ID,
                        CreateTime = DateTime.Now,
                        PhotoCount = 0,
                        UpdateTime= DateTime.Now
                    };
                    UnitOfWork.Repository<SYS_Album>().AddEntity(album);
                }
                var isOK= UnitOfWork.CommitAsync().Result;
                if (isOK > 0)
                    mjRet.code = 1;
            }
            return mjRet;
        }
        public ModelJsonRet AddSingleAlbum(string name, byte type, int classId, long stuId)
        {
            object objClassId = null;
            if (classId > 0)
            {
                objClassId = classId;
            }
            object masterId = null;
            if (stuId > 0)
            {
                var stu = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.ID == stuId).Result.FirstOrDefault();
                if(stu!=null)
                {
                    masterId = stuId;
                    objClassId = stu.ClassId;
                }
            }
            var album = new SYS_Album
            {
                SchoolId = mlUser.School.ID,
                AlbumName = name,
                Type = type,
                ClassId = (int?)objClassId,
                MasterId = (int?)masterId,
                CreateTime = DateTime.Now,
                PhotoCount = 0,
                UpdateTime = DateTime.Now
            };
            UnitOfWork.Repository<SYS_Album>().AddEntity(album);
            mjRet.code=(byte)UnitOfWork.CommitAsync().Result;
            return mjRet;
        }

        public ModelJsonRet GetNoAlbumStus()
        {
            var sql = "select*from SYS_Student where Status=1 and SchoolId=" + mlUser.School.ID + 
                " and ID not in(select s.ID from SYS_Student as s inner join SYS_Album as a on s.ID=a.MasterId where s.Status=1 and s.SchoolId=" + mlUser.School.ID+")";
            var allNoAlbumStus = UnitOfWork.ExecuteSqlQuery<SYS_Student>(sql).ToList();
            mjRet.code = 1;
            mjRet.content = allNoAlbumStus;
            return mjRet;
        }
        public ModelJsonRet GetAlbumsInfo(string nQuery, int cQuery, int pageIndex)
        {
            //var test = UnitOfWork.Repository<SYS_Student>().GetEntities(x => true).ToList();
            //var stus= UnitOfWork.Repository<SYS_Student>().GetEntities(x=>true).OrderByDescending(x=>x.CreateTime).ThenBy(x=>x.ClassId).Skip(2).Take(8).ToList();
            var pageItemCount = 20;

            var totalCount = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID).Result.Count();
            var albums = UnitOfWork.Repository<SYS_Album>().GetEntitiesForPageOrderByDescAsync(x => x.SchoolId == mlUser.School.ID, x => x.CreateTime,
                (pageIndex - 1) * pageItemCount, pageItemCount).Result;
            if (nQuery!=null&&nQuery.Length > 0)
            {
                totalCount = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && (x.AlbumName.Contains(nQuery))).Result.Count();
                albums = UnitOfWork.Repository<SYS_Album>().GetEntitiesForPageOrderByDescAsync(x => x.SchoolId == mlUser.School.ID && (x.AlbumName.Contains(nQuery)), x => x.CreateTime,
                (pageIndex - 1) * pageItemCount, pageItemCount).Result;
            }
            if (cQuery > 0)
            {
                totalCount = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && (x.ClassId == cQuery)).Result.Count();
                albums = UnitOfWork.Repository<SYS_Album>().GetEntitiesForPageOrderByDescAsync(x => x.SchoolId == mlUser.School.ID && (x.ClassId == cQuery), x => x.CreateTime,
                (pageIndex - 1) * pageItemCount, pageItemCount).Result;
            }
            if (cQuery < 0)
            {
                if (cQuery == -1)//学校
                {
                    totalCount = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.Type==(byte)AlbumType.学校相册).Result.Count();
                    albums = UnitOfWork.Repository<SYS_Album>().GetEntitiesForPageOrderByDescAsync(x => x.SchoolId == mlUser.School.ID && x.Type == (byte)AlbumType.学校相册, x => x.CreateTime,
                (pageIndex - 1) * pageItemCount, pageItemCount).Result;
                }
                else
                {
                    totalCount = UnitOfWork.Repository<SYS_Album>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.Type == (byte)AlbumType.班级相册).Result.Count();
                    albums = UnitOfWork.Repository<SYS_Album>().GetEntitiesForPageOrderByDescAsync(x => x.SchoolId == mlUser.School.ID && x.Type == (byte)AlbumType.班级相册, x => x.CreateTime,
                (pageIndex - 1) * pageItemCount, pageItemCount).Result;
                }
            }

            var classes= (List<SYS_Class>)GetSchoolEntities("class");

            //var allNoAlbumStus= UnitOfWork.Repository<SYS_Student>().GetEntities(x=>x.SchoolId==mlUser.School.ID&& x.Status == (byte)StuStatus.正常).OrderBy(x => x.Grade).ThenBy(x => x.ClassId).ToList();
            var totalPage = 1;
            if (totalCount > pageItemCount)
            {
                totalPage = totalCount / pageItemCount + 1;
            }
            var tipStr = "无相册，请添加";
            if (nQuery != null&&nQuery.Length > 0)
                tipStr = "未找到相册";
            var sb = new StringBuilder("<div style='margin-left:50%;'><h4 class='text-danger'>"+ tipStr + "</h4></div>");
            if (albums.Count > 0)
            {
                sb = new StringBuilder();
                foreach (var item in albums)
                {
                    var photoCount = item.PhotoCount;
                    var name = item.AlbumName;
                    var imgUrl = item.Preview;
                    //if (photoCount <= 0)
                    //{
                    //    imgUrl = "/Images/null_Album.jpg";
                    //}
                    sb.Append("<div class='box album-summary'>");
                    sb.Append("<div class='box-body' onclick='goToAlbum("+item.ID+")' style='background-image:url("+ imgUrl + ")'>");
                    sb.Append(" <div><b>"+ photoCount + "</b></div></div>");
                    sb.Append("<div class='box-footer'><b class='text-success'>"+ name + "</b> ▪ " + ((AlbumType)item.Type).ToString().Substring(0,2)+
                        "<i class='fa fa-trash' onclick='delAlbum("+ item.ID + ")'></i></div></div>");
                }
            }
            var json = new {
                totalPage = totalPage,
                classes = classes,
                albumStr = sb.ToString()
            };
            mjRet.code = 1;
            mjRet.content = json;
            return mjRet;
        }
        #endregion
    }
}
