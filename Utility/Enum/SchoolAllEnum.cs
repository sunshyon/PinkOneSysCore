using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    #region 人员相关
    public enum StuStatus
    {
        正常 = 1,
        毕业 = 2,
        修学 = 3,
        退学 = 4,
        预报名 = 5,
    }
  
    public enum GradeEnum
    {
        托班 = 1,
        小班 = 2,
        中班 = 3,
        大班 = 4
    }

    public enum RelationType
    {
        爸爸 = 1,
        妈妈 = 2,
        爷爷 = 3,
        奶奶 = 4,
        姥爷 = 5,
        姥姥 = 6,
        其他 = 7,
    }
    public enum StaffStatus
    {
        在职 = 1,
        离职 = 2,
    }
    
    #endregion

    #region  考勤相关
    /// <summary>
    /// 考勤类型
    /// </summary>
    public enum AttType
    {
        签入 = 1,
        签出 = 2,
        体测 = 3,
        其他 = 4
    }
    /// <summary>
    /// 人员此刻出勤状态
    /// </summary>
    public enum CurrentAttStatus
    {
        在校 = 1,
        离校 = 2,
        未签到 = 3,
    }
    /// <summary>
    /// 考勤方式
    /// </summary>
    public enum AttWay
    {
        刷卡 = 1,
        扫码 = 2,
        手工添加 = 3,
        其他=4
    }
    #endregion

    #region 卡相关
    public enum CardType
    {
        管理员卡=1,
        学生卡=2,
        职员卡=3,
    }
    public enum CardStatus
    {
        正常 = 1,
        挂失 = 2,
        注销 = 3,
    }
    #endregion

    #region 学校相关
    public enum SchoolType
    {
        幼儿园=1,
        小学=2,
    }
    public enum SchoolStatus
    {
        正常 = 1,
        试用 = 2,
        暂停使用 = 3,
        注销 = 4,
    }
    public enum NoticeType
    {
        陪绮发给学校 = 1,
        陪绮发给家长 = 2,
        学校内部 = 3,
        学校发给家长 = 4,
    }
    #endregion

    #region 相册相关
    public enum AlbumType
     {
        学校相册 = 1,
        班级相册 = 2,
        个人相册 = 3,
    }
    #endregion
}
