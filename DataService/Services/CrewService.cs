using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class CrewService:BaseService,ICrewService
    {
        /// <summary>
        /// 批量导入学生
        /// </summary>
        public  string ImportStus(int classId, string fileName)
        {
            var res = "数据格式有误";
            var totalRowCount = 0;
            var stuCount = 0;
            var repeatStuStr = "";
            var repeatCardStr = "";
            try
            {
                DataTable dt = ExcelHelper.ImportExcel(fileName);
                var nameCol = 0;
                var isNameColOk = false;
                var cls = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.ID == classId && x.SchoolId == mlUser.School.ID).Result.FirstOrDefault();
                if (null != cls && dt.Rows.Count > 1)
                {
                    var allStus = (List<SYS_Student>)GetSchoolEntities("stu");
                    totalRowCount = dt.Rows.Count - 1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        if (!isNameColOk)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                //定位姓名所在列号
                                if (dr[j].ToString().Contains("姓名"))
                                {
                                    nameCol = j;
                                    isNameColOk = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //新建学生
                            byte sex = 1;
                            var name = dr[nameCol].ToString();
                            var phone = dr[nameCol + 1].ToString();
                            var oldStu = allStus.Where(x => x.StuName == name && x.Phone == phone&&x.SchoolId==mlUser.School.ID&&x.Status==(byte)StuStatus.正常).FirstOrDefault();
                            if (oldStu == null)
                            {
                                if (dr[nameCol + 2].ToString().Contains("女"))
                                    sex = 2;
                                var stu = new SYS_Student
                                {
                                    StuName = name,
                                    SchoolId = mlUser.School.ID,
                                    ClassId = classId,
                                    Grade = cls.Grade,
                                    Phone = phone,
                                    Sex = sex,
                                    StuNo = dr[nameCol + 3].ToString(),
                                    Address = dr[nameCol + 4].ToString(),
                                    Birthday = DateTime.Parse(dr[nameCol + 5].ToString()),
                                    Status = (byte)StuStatus.正常,
                                    AttStatus = (byte)CurrentAttStatus.未签到,
                                    CreateTime = DateTime.Now
                                };
                                UnitOfWork.Repository<SYS_Student>().AddEntity(stu);
                                stuCount++;

                                //新建卡
                                var totalCol = dr.ItemArray.Count();
                                if (totalCol > 5)
                                {
                                    var cardNoList = new List<string>();

                                    for (var k = 1; k < totalCol - 5; k++)
                                    {
                                        int cardNo = 0;
                                        var cardNoStr = dr[nameCol + 5 + k].ToString();
                                        if (cardNoStr.Length > 3 && int.TryParse(cardNoStr, out cardNo))
                                            cardNoList.Add(cardNoStr);
                                        else
                                            break;
                                    }
                                    if (cardNoList.Count > 0)
                                    {
                                        //先提交学生到数据库-->得到学生ID
                                        var isOk = UnitOfWork.CommitAsync().Result;
                                        if (isOk > 0)
                                        {
                                            //开始建卡
                                            foreach (var cNo in cardNoList)
                                            {
                                                var oldCard = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo == cNo && x.SchoolId == mlUser.School.ID && x.Status == (byte)CardStatus.正常).Result
                                                    .FirstOrDefault();
                                                if (oldCard == null)
                                                {
                                                    var card = new SYS_Card
                                                    {
                                                        SchoolId = mlUser.School.ID,
                                                        CardMasterId = stu.ID,
                                                        CardNo = cNo,
                                                        CardType = (byte)CardType.学生卡,
                                                        Status = (byte)CardStatus.正常,
                                                        CreateTime = DateTime.Now
                                                    };
                                                    UnitOfWork.Repository<SYS_Card>().AddEntity(card);
                                                }
                                                else
                                                {
                                                    repeatCardStr += stu.StuName + ":" + cNo + "  ";
                                                }
                                            }
                                            UnitOfWork.Commit();
                                        }
                                    }
                                }
                            }
                            else//同一个学校学生信息重复
                            {
                                repeatStuStr += oldStu.StuName+"  ";
                            }
                            
                        }
                    }
                    UnitOfWork.Commit();
                    if (repeatStuStr.Length > 0)
                        repeatStuStr = "<b class='text-danger'>" + repeatStuStr + "</b>";
                    else
                        repeatStuStr = "无";
                    if (repeatCardStr.Length > 0)
                        repeatCardStr = "<b class='text-danger'>" + repeatCardStr + "</b>";
                    else
                        repeatCardStr = "无";

                    res = "<div>操作OK，完成导入学生数：<b class='text-success'>" + stuCount +
                        "</b></div><div>重复学生导入失败：" + repeatStuStr+
                    "</div><div>重复卡号建卡失败：" + repeatCardStr + "</div>";

                    
                }
                dt.Dispose();
            }
            catch(Exception ex)
            {
                res = "错误：" + ex.Message;
            }
           
            GC.Collect();
            System.IO.File.Delete(fileName);
            return res;
        }
             

        /// <summary>
        /// 获取学生详情Model
        /// </summary>
        public ModelStuDetail GetStuDetailModel(long stuId)
        {
            ModelStuDetail res = new ModelStuDetail();
            var allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            var stu = allStus.Where(x => x.ID == stuId).FirstOrDefault();
            if (null != stu)
            {
                var clas = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.ID == stu.ClassId).Result.FirstOrDefault();
                var fks = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.StuId == stu.ID).Result;
                var parents = new List<SYS_Parent>();
                if (fks.Count > 0)
                {
                    foreach (var fk in fks)
                    {
                        var parent = UnitOfWork.Repository<SYS_Parent>().GetEntitiesAsync(x => x.ID == fk.ParentId).Result.FirstOrDefault();
                        if (parent != null)
                            parents.Add(parent);
                    }
                }
                var cards = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.CardMasterId == stu.ID && x.CardType == (byte)CardType.学生卡).Result;

                res.Stu = stu;
                res.Class = clas;
                res.Parents = parents;
                res.Cards = cards;
            }
            return res;
        }


        /// <summary>
        /// 获取学生信息
        /// </summary>
        public string GetStuInfo(string nqy, int cqy)
        {
            var res = "";
            List<SYS_Student> allStus= (List<SYS_Student>)GetSchoolEntities("stu");
            if (null != nqy && nqy.Length > 0)
                allStus = allStus.Where(x => x.StuName.Contains(nqy)).ToList();
            if (cqy > 0)
                allStus = allStus.Where(x => x.ClassId == cqy).ToList();
            List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");

            if (allStus.Count > 0|| classes.Count>0)
            {
                var json = new {
                    Stus=allStus,
                    Classes= classes
                };

                res = JsonHelper.ToJson(json);
            }
            return res;
        }


        /// <summary>
        /// 新增或修改学生(type->1:新增,2:修改)
        /// </summary>
        public string AddOrModifyStu(byte type, string entity)
        {
            var res = "操作失败";
            if (entity.Length > 6)
            {
                List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");
                SYS_Student obj = JsonHelper.JsonToTByKey<SYS_Student>(entity,"Student");
                List<string> cardNos= JsonHelper.JsonToTByKey<List<string>>(entity, "Cards");
               
                if (null != obj)
                {
                    SYS_Class clas = classes.Where(x => x.ID == obj.ClassId).FirstOrDefault();
                    if (type == 1)//新增
                    {
                        obj.SchoolId = mlUser.School.ID;
                        obj.Status = (byte)StuStatus.正常;
                        obj.Grade = clas.Grade;
                        obj.AttStatus = (byte)CurrentAttStatus.未签到;
                        obj.CreateTime = DateTime.Now;

                        var oldStu = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.StuName == obj.StuName && x.Phone == obj.Phone && x.SchoolId == obj.SchoolId&&x.Status==(byte)StuStatus.正常).Result.FirstOrDefault();
                        var isOk = 0;
                        if (oldStu == null)
                        {
                            UnitOfWork.Repository<SYS_Student>().AddEntity(obj);
                            isOk = UnitOfWork.CommitAsync().Result;
                            if (isOk > 0)
                                res = "OK,学生添加成功";
                        }
                        else
                        {
                            res = "学生信息重复，无法添加\r\n";
                        }
                        //建卡
                        var stuOldCards = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardMasterId == obj.ID && x.CardType==(byte)CardType.学生卡).Result;//该学生已有卡
                        if (isOk > 0 && null != cardNos && cardNos.Count > 0)
                        {
                            var repeatCardStr = "";
                            foreach (var cNo in cardNos)
                            {
                                if (stuOldCards.FindIndex(delegate (SYS_Card card) { return card.CardNo == cNo; }) < 0)
                                {
                                    var oldCard = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo == cNo && x.SchoolId == mlUser.School.ID && x.Status == (byte)CardStatus.正常)
                                        .Result.FirstOrDefault();
                                    if (oldCard == null)
                                    {
                                        SYS_Card newCard = new SYS_Card
                                        {
                                            CardNo = cNo,
                                            CardMasterId = obj.ID,
                                            SchoolId = mlUser.School.ID,
                                            CardType = (byte)CardType.学生卡,
                                            Status = (byte)CardStatus.正常,
                                            CreateTime = DateTime.Now
                                        };
                                        UnitOfWork.Repository<SYS_Card>().AddEntity(newCard);
                                        isOk = UnitOfWork.CommitAsync().Result;
                                    }
                                    else
                                    {
                                        repeatCardStr += cNo + " ";
                                    }
                                }
                            }
                            if(repeatCardStr.Length>0)
                                res = "警告：学生添加成功，但是卡" + repeatCardStr + "已存在，无法添加";
                        }
                    }
                    else//修改
                    {
                        UnitOfWork.Repository<SYS_Student>().UpdateEntity(obj);
                        var isOk = UnitOfWork.CommitAsync().Result;
                        if (isOk > 0)
                            res = "OK,学生修改成功";
                        else
                            res = "学生信息修改失败\r\n";
                        ///建卡
                        //检索该学生已有卡
                        var stuOldCards = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardMasterId == obj.ID && x.CardType == (byte)CardType.学生卡).Result;
                        if (isOk>0&&null != cardNos && cardNos.Count > 0)
                        {
                            var repeatCardStr = "";
                            foreach (var cNo in cardNos)
                            {
                                if (stuOldCards.FindIndex(delegate (SYS_Card card) { return card.CardNo == cNo; }) < 0)
                                {
                                    var oldCard = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo == cNo && x.SchoolId == mlUser.School.ID && x.Status == (byte)CardStatus.正常)
                                        .Result.FirstOrDefault();
                                    if (oldCard == null)
                                    {
                                        SYS_Card newCard = new SYS_Card
                                        {
                                            CardNo = cNo,
                                            CardMasterId = obj.ID,
                                            SchoolId = mlUser.School.ID,
                                            CardType = (byte)CardType.学生卡,
                                            Status = (byte)CardStatus.正常,
                                            CreateTime = DateTime.Now
                                        };
                                        UnitOfWork.Repository<SYS_Card>().AddEntity(newCard);
                                        isOk = UnitOfWork.CommitAsync().Result;
                                    }
                                    else
                                    {
                                        repeatCardStr += cNo + " ";
                                    }
                                }

                            }
                            if (repeatCardStr.Length > 0)
                                res = "警告：卡" + repeatCardStr + "已存在，无法添加";
                        }
                    }
                }
                
            }
            return res;
        }
        /// <summary>
        /// 通过ID获取Stu
        /// </summary>
        public string GetStuById(int id)
        {
            var res = "";
            var entity = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
            if (null != entity)
            {
                //var cardNo
                List<SYS_Card> cards = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardMasterId == entity.ID && x.CardType == (byte)CardType.学生卡).Result.OrderBy(x=>x.ID).ToList();
                List<string> cardStr = new List<string>();
                if (null!= cards && cards.Count > 0)
                {
                    foreach(var item in cards)
                    {
                        cardStr.Add(item.CardNo);
                    }
                }
                var json = new
                {
                    Student = entity,
                    Cards= cardStr
                };
                res = JsonHelper.ToJson(json);
            }
            return res;
        }
        /// <summary>
        /// 通过Id删除实体
        /// </summary>
        public int DelStuById(long id)
        {
            var res = 0;
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            SYS_Student obj = allStus.Where(x=>x.ID==id).FirstOrDefault();
            if (null != obj)
            {
                UnitOfWork.Repository<SYS_Student>().DeleteEntity(obj);
                res = UnitOfWork.CommitAsync().Result;
                
            }
            return res;
        }

        #region 职员相关
        /// <summary>
        /// 批量导入职员
        /// </summary>
        public string ImportStaffs(string fileName)
        {
            var res = "数据格式有误";
            var totalRowCount = 0;
            var staffCount = 0;
            var repeatStaffStr = "";
            var repeatCardStr = "";
            try
            {
                DataTable dt = ExcelHelper.ImportExcel(fileName);
                var nameCol = 0;
                var isNameColOk = false;
                if ( dt.Rows.Count > 1)
                {
                    var staffs = (List<SYS_Staff>)GetSchoolEntities("staff");
                    totalRowCount = dt.Rows.Count - 1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        if (!isNameColOk)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                //定位姓名所在列号
                                if (dr[j].ToString().Contains("姓名"))
                                {
                                    nameCol = j;
                                    isNameColOk = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //新建职员
                            var name = dr[nameCol].ToString();
                            var phone = dr[nameCol + 1].ToString();
                            var roleName= dr[nameCol + 2].ToString();
                            var role = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.RoleName == roleName).Result.FirstOrDefault();
                            //检索是否重复
                            var oldStaff= staffs.Where(x => x.StaffName == name && x.Phone == phone && x.SchoolId == mlUser.School.ID && x.Status == (byte)StuStatus.正常).FirstOrDefault();
                            if (oldStaff == null)
                            {
                                var staff = new SYS_Staff
                                {
                                    StaffName = name,
                                    SchoolId = mlUser.School.ID,
                                    OpenId = Guid.NewGuid().ToString(),
                                    WorkNo = dr[nameCol + 3].ToString(),
                                    Phone = phone,
                                    Status = (byte)StaffStatus.在职,
                                    AttStatus = (byte)CurrentAttStatus.未签到,
                                    CreateTime = DateTime.Now
                                };
                                if (role != null)
                                {
                                    staff.RoleId = role.ID;
                                    staff.RoleLevel = role.RoleLevel;
                                }
                                UnitOfWork.Repository<SYS_Staff>().AddEntity(staff);
                                staffCount++;

                                //新建卡
                                var totalCol = dr.ItemArray.Count();
                                if (totalCol- nameCol > 4)
                                {
                                    int cardNo = 0;
                                    var cardNoStr = dr[nameCol + 4].ToString();
                                    if (cardNoStr.Length > 3&&int.TryParse(cardNoStr,out cardNo))
                                    {
                                        //先提交学生到数据库-->得到学生ID
                                        var isOk = UnitOfWork.CommitAsync().Result;
                                        if (isOk > 0)
                                        {
                                            //开始建卡
                                            var oldCard = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo == cardNoStr && x.SchoolId == mlUser.School.ID && x.Status == (byte)CardStatus.正常)
                                                .Result.FirstOrDefault();
                                            if (oldCard == null)
                                            {
                                                var card = new SYS_Card
                                                {
                                                    SchoolId = mlUser.School.ID,
                                                    CardMasterId = staff.ID,
                                                    CardNo = cardNoStr,
                                                    CardType = (byte)CardType.职员卡,
                                                    Status = (byte)CardStatus.正常,
                                                    CreateTime = DateTime.Now
                                                };
                                                UnitOfWork.Repository<SYS_Card>().AddEntity(card);
                                            }
                                            else
                                            {
                                                repeatCardStr += staff.StaffName + ":" + cardNoStr + "  ";
                                            }
                                            UnitOfWork.Commit();
                                        }
                                    }
                                }
                            }
                            else//同一个学校职员信息重复
                            {
                                repeatStaffStr += oldStaff.StaffName + "  ";
                            }

                        }
                    }
                    UnitOfWork.Commit();
                    if (repeatStaffStr.Length > 0)
                        repeatStaffStr = "<b class='text-danger'>" + repeatStaffStr + "</b>";
                    else
                        repeatStaffStr = "无";
                    if (repeatCardStr.Length > 0)
                        repeatCardStr = "<b class='text-danger'>" + repeatCardStr + "</b>";
                    else
                        repeatCardStr = "无";

                    res = "<div>操作OK，完成导入职员数：<b class='text-success'>" + staffCount +
                        "</b></div><div>重复导入失败：" + repeatStaffStr +
                    "</div><div>重复卡号建卡失败：" + repeatCardStr + "</div>";

                   
                }
                dt.Dispose();
            }
            catch (Exception ex)
            {
                res = "错误：" + ex.Message;
            }

            GC.Collect();
            System.IO.File.Delete(fileName);
            return res;
        }
        public ModelJsonRet AddStaffPinkoneAccount(int staffId, string account,string password)
        {
            mjRet.errMsg = "创建失败";
            var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.ID == staffId).Result.FirstOrDefault();
            if (staff != null)
            {
                var hasAcut = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.PinkoneAccount == account&&x.Status==(byte)StaffStatus.在职).Result.FirstOrDefault();
                if (hasAcut == null)
                {
                    staff.PinkoneAccount = account;
                    staff.PinkonePassword = password;
                    UnitOfWork.Repository<SYS_Staff>().UpdateEntity(staff);
                    var isOK = UnitOfWork.CommitAsync().Result;
                    if (isOK > 0)
                    {
                        mjRet.code = 1;
                        mjRet.content = "OK";
                    }
                }
                else
                    mjRet.errMsg = "该账户已经存在，无法创建";
            }
            return mjRet;
        }
        public ModelStaffDetail GetStaffDetailModel(int staffId)
        {
            var msd = new ModelStaffDetail();
            var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.ID == staffId).Result.FirstOrDefault();
            if (staff != null)
            {
                var role = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.ID == staff.RoleId).Result.FirstOrDefault();
                var card = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.CardType == (byte)CardType.职员卡 && x.CardMasterId == staff.ID).Result.FirstOrDefault();
                if(card==null && staff.CardNo !=null&& staff.CardNo.Length > 0)
                {
                    staff.CardNo = null;
                    UnitOfWork.Repository<SYS_Staff>().UpdateEntity(staff);
                    UnitOfWork.CommitAsync();
                }
                msd.Staff = staff;
                msd.Role = role;
                msd.Card = card;
            }
            return msd;
        }

        /// <summary>
        /// 获取职员信息
        /// </summary>
        public string GetStaffInfo(string query)
        {
            var res = "";
            List<SYS_Staff> staffs = (List<SYS_Staff>)GetSchoolEntities("staff");
            if (null != query && query.Length > 0)
                staffs = staffs.Where(x => x.StaffName.Contains(query) || x.Phone.Contains(query)).ToList();

            if (null != staffs && staffs.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                //角色管理
                var staffRoles = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID || x.SchoolId == 0).Result.OrderBy(x=>x.RoleLevel).ToList();
                var cards = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID&& x.CardType==(byte)CardType.职员卡 && x.Status == (byte)CardStatus.正常).Result;
                foreach (var item in staffs)
                {
                    var card = cards.Where(x => x.CardMasterId == item.ID).FirstOrDefault();
                    var role = staffRoles.Where(x => x.ID == item.RoleId).FirstOrDefault();
                    sb.Append("<tr>");
                    sb.Append("<td>" + item.StaffName + "</td>");
                    sb.Append("<td>" + item.Phone + "</td>");
                    sb.Append("<td>" + item.WorkNo + "</td>");
                    sb.Append("<td>" + (role == null ? "未填" : role.RoleName) + "</td>");
                    sb.Append("<td>" + (card==null?"无":card.CardNo) + "</td>");
                    string operateStr = "<span class='span-as-btn' onclick='modifyStaff(" + item.ID + ")'>修改</span>" +
                     "<span class='span-as-btn' onclick='delStaff(" + item.ID + ")'>删除</span>"+
                     "<span class='span-as-btn' onclick='openStaffDetailPage(" + item.ID + ")'>详情</span>";
                    sb.Append("<td>" + operateStr + "</td>");
                    sb.Append("</tr>");
                }
                var json = new
                {
                    Staffs = sb.ToString(),
                    StaffRoles = staffRoles
                };
                res = JsonHelper.ToJson(json);
            }
            return res;
        }
        /// <summary>
        /// 新增或修改职员(type->1:新增,2:修改)
        /// </summary>
        public string AddOrModifyStaff(byte type, string entity)
        {
            var res = "";
            var repeatCardStr = "";
            if (entity.Length > 6)
            {
                SYS_Staff obj = JsonHelper.JsonToT<SYS_Staff>(entity);
                if (null != obj)
                {
                    var role = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.ID == obj.RoleId).Result.FirstOrDefault();
                    if (null != role)
                        obj.RoleLevel = role.RoleLevel;

                    if (type == 1)
                    {
                        obj.SchoolId = mlUser.School.ID;
                        obj.OpenId = Guid.NewGuid().ToString();
                        obj.Status = (byte)StaffStatus.在职;
                        obj.CreateTime = DateTime.Now;

                        var oldStaff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.StaffName == obj.StaffName && x.Phone == obj.Phone && x.SchoolId == obj.SchoolId && x.Status == (byte)StaffStatus.在职).Result.FirstOrDefault();
                        var isOk = 0;
                        if (oldStaff == null)
                        {
                            UnitOfWork.Repository<SYS_Staff>().AddEntity(obj);
                            isOk = UnitOfWork.CommitAsync().Result;
                            if (isOk > 0)
                                res = "OK,职员添加成功";
                        }
                        else
                        {
                            res = "职员信息重复，无法添加";
                        }

                        //添加卡
                        if(isOk > 0&& obj.CardNo != null && obj.CardNo.Length > 0)
                        {
                            //检测卡是否已存在
                            var oldCard = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo== obj.CardNo&& x.SchoolId==mlUser.School.ID&&x.Status==(byte)CardStatus.正常).Result.FirstOrDefault();
                            if (oldCard == null)
                            {
                                var card = new SYS_Card
                                {
                                    CardNo = obj.CardNo,
                                    CardMasterId = obj.ID,
                                    SchoolId = mlUser.School.ID,
                                    CardType = (byte)CardType.职员卡,
                                    CreateTime = DateTime.Now,
                                    Status = (byte)CardStatus.正常
                                };
                                UnitOfWork.Repository<SYS_Card>().AddEntity(card);
                                isOk = UnitOfWork.CommitAsync().Result;
                                if (isOk > 0)
                                    res = "OK";
                            }
                            else
                            {
                                repeatCardStr = obj.CardNo;
                            }
                            if (repeatCardStr.Length > 0)
                                res = "警告：职员添加成功，但是卡" + repeatCardStr + "已存在，无法添加";
                        }
                    }
                    //修改职员
                    else
                    {
                        UnitOfWork.Repository<SYS_Staff>().UpdateEntity(obj);
                        res = "OK,修改成功";
                        //添加卡
                        if (obj.CardNo != null && obj.CardNo.Length > 0)
                        {
                            //检测该卡号是否已存在
                            var oldCard = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo == obj.CardNo && x.SchoolId == mlUser.School.ID && x.Status == (byte)CardStatus.正常).Result.FirstOrDefault();
                            if (oldCard == null)
                            {
                                //该用户是否有老卡,有则先注销
                                //var staffOldCard = UnitOfWork.Repository<SYS_Card>().GetEntities(x => x.CardMasterId == obj.ID && x.CardType == (byte)CardType.职员卡).FirstOrDefault();
                                //if (staffOldCard != null)
                                //{
                                //    staffOldCard.Status = (byte)CardStatus.注销;
                                //    UnitOfWork.Repository<SYS_Card>().UpdateEntity(staffOldCard);
                                //}
                                var card = new SYS_Card
                                {
                                    CardNo = obj.CardNo,
                                    CardMasterId = obj.ID,
                                    SchoolId = mlUser.School.ID,
                                    CardType = (byte)CardType.职员卡,
                                    CreateTime = DateTime.Now,
                                    Status = (byte)CardStatus.正常
                                };
                                UnitOfWork.Repository<SYS_Card>().AddEntity(card);
                            }
                            else
                            {
                                repeatCardStr = obj.CardNo;
                                obj.CardNo = "";
                            }
                            if (repeatCardStr.Length > 0)
                                res = "警告：卡" + repeatCardStr + "已存在，无法添加";
                        }
                        var isOk= UnitOfWork.CommitAsync().Result;
                        if (isOk > 0)
                            res = "OK";
                    }
                   
                }
                UnitOfWork.Commit();
               
            }
            return res;
        }
        /// <summary>
        /// 通过ID获取Staff
        /// </summary>
        public string GetStaffById(int id)
        {
            var res = "";
            var entity = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
            if (null != entity)
            {
                if (entity.CardNo!=null&&entity.CardNo.Length > 0)
                { 
                    //检索名下是否有正常的卡
                    var card = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo == entity.CardNo && x.CardMasterId == entity.ID && x.Status == (byte)CardStatus.正常 && x.SchoolId == mlUser.School.ID).Result.FirstOrDefault();
                    if (card == null)
                    {
                        entity.CardNo = "";
                        UnitOfWork.Repository<SYS_Staff>().UpdateEntity(entity);
                        UnitOfWork.Commit();
                    }
                }
                res = JsonHelper.ToJson(entity);
            }
                
            return res;
        }
        /// <summary>
        /// 通过Id删除实体
        /// </summary>
        public int DelStaffById(long id)
        {
            var res = 0;
            SYS_Staff obj = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x=>x.ID==id).Result.FirstOrDefault();
            if (null != obj)
            {
                UnitOfWork.Repository<SYS_Staff>().DeleteEntity(obj);
                res = UnitOfWork.CommitAsync().Result;
               
            }
            return res;
        }

        #endregion

        /// <summary>
        /// 获取班级信息
        /// </summary>
        public string GetClassInfo()
        {
            var res="";

            List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");
            List<SYS_Staff> staffs = (List<SYS_Staff>)GetSchoolEntities("staff");
            staffs = staffs.Where(x => x.RoleLevel>=1&& x.RoleLevel<4).ToList();
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
           
            if (classes.Count > 0|| staffs.Count>0)
            {
                StringBuilder sb = new StringBuilder();
               
                foreach (var item in classes)
                {
                    string operateStr = "<span class='span-as-btn' onclick='modifyClass(" + item.ID + ")'>修改</span>" +
                    "<span class='span-as-btn' onclick='delClass(" + item.ID + ")'>删除</span>";

                    var classTeas = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == item.SchoolId && x.Status == (byte)StaffStatus.在职 &&
                      (x.ID == item.ClassTeacherId1 || x.ID == item.ClassTeacherId2 || x.ID == item.ClassTeacherId3 || x.ID == item.ClassTeacherId4)).Result;
                    var classTeasStr = "未选择";
                    if (classTeas.Count > 0)
                    {
                        classTeasStr = "";
                        foreach (var t in classTeas)
                        {
                            classTeasStr += t.StaffName + " ";
                        }
                    }
                    sb.Append("<tr>");
                    sb.Append("<td>" + item.ClassName + "</td>");
                    sb.Append("<td>" + (GradeEnum)item.Grade + "</td>");
                    sb.Append("<td>" + classTeasStr + "</td>");
                    List<SYS_Student> cStus = allStus.Where(x => x.ClassId == item.ID).ToList();
                    sb.Append("<td>" + cStus.Count + "</td>");
                    if (item.Grade == (byte)GradeEnum.大班)
                        operateStr += "<span class='span-as-btn' onclick='updateClass(" + item.ID + ",1)'>毕业</span>";
                    else
                        operateStr += "<span class='span-as-btn' onclick='updateClass(" + item.ID + ",2)'>升班</span>";
                   
                   sb.Append("<td>" + operateStr+ "</td>");
                   sb.Append("</tr>");
                }
                var classesStr= sb.ToString();
                var json = new
                {
                    classStr= sb.ToString(),
                    techers= staffs
                };
                res = JsonHelper.ToJson(json);
            }
            return res;
        }
        /// <summary>
        /// 新增或修改班级(type->1:新增,2:修改)
        /// </summary>
        public int AddOrModifyClass(byte type, string entity)
        {
            var res = 0;
            if (entity.Length > 6)
            {
                List<SYS_Class> clases= (List<SYS_Class>)GetSchoolEntities("class");
                SYS_Class obj = JsonHelper.JsonToT<SYS_Class>(entity);
               
                if (null != obj)
                {
                    if (type == 1)
                    {
                        obj.SchoolId = mlUser.School.ID;
                        obj.Status = 1;
                        obj.ClassNo = (clases.Count + 1).ToString();

                        UnitOfWork.Repository<SYS_Class>().AddEntity(obj);
                    }
                    else
                    {
                        UnitOfWork.Repository<SYS_Class>().UpdateEntity(obj);
                    }
                }
                res = UnitOfWork.CommitAsync().Result;
           
            }
            return res;
        }
        /// <summary>
        /// 通过ID获取Class
        /// </summary>
        public string GetClassById(int id)
        {
            var res = "";
            var entity = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x=>x.ID==id).Result.FirstOrDefault();
            if (null != entity)
                res = JsonHelper.ToJson(entity);
            return res;
        }
        /// <summary>
        /// 通过Id删除实体
        /// </summary>
        public int DelClassById(long id)
        {
            var res = 0;
            SYS_Class obj = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x=>x.ID==id).Result.FirstOrDefault();
            if (null != obj)
            {
                UnitOfWork.Repository<SYS_Class>().DeleteEntity(obj);
                res = UnitOfWork.CommitAsync().Result;
               
            }
            return res;
        }

        
    }
}
