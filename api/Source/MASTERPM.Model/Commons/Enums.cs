using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASTERPM.Model.Commons
{
    public enum EStatus
    {
        Fail = 0,
        Ok = 1
    }
	

    public enum EStatusFile
    {
        OverWeight = 0,
        PathNotExist = 1,
        Base64Incorrect = 2,
    }

    public enum EAction
    {
        Get,
        Insert,
        Update,
        Delete,
        Approve,
        UnApprove,
        Reject,
        ModifyApprove,
        Remind,
    }


    public enum EGender
    {
        Male,
        Female,
        Other
    }



    public enum ECommon
    {
        Post_Reference = 0, // nguồn tin bài
        Position = 1, // chức vụ
        Ethnic = 2, // Dân tộc
        PoliticalTheory = 3, // Lý luận chính trị
        CommunistPartyPosition = 4, // Chức vụ đảng
        UnitType = 5, // Phân loại đơn vị (khối,...)
        Qualification = 6,// trình độ chuyên môn
    }
    public enum EConfig
    {
        TempForgotPassword,
        RoleDefault,
        APPROVERS
    }

    public enum EAutoNumber
    {
        PDF_CODE,
    }


    public enum ERewardStatus
    {
        Waiting = 0, // chờ gửi
        Send = 1, // đã gửi
        Approved = 2, // đã duyệt
        OCR = 3 // đã OCR
    }



}
