using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Commons
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
        Release,
        Pay,
        Cancel,
        Signed,
        Unsigned
    }


    public enum EGender
    {
        Male,
        Female,
        Other
    }


    public enum ETypeMember
    {
        PersonalID = 1,
        UnitID = 2,
        Other = 3
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
        MailServer,
        TempForgotPassword,
        RoleDefault,
        RegistSpeak_CheckDate,
        MeetingComment_CheckDate,
        GetConfigRegist,
        ShowPopupWhenCheckIn,
        ShowMobileUserRegistScreen,
        APPROVERS
    }

    public enum EAutoNumber
    {
        PDF_CODE,
    }

    public enum EUnitUsed
    {
        VTTP
    }
   
    public enum ECvHistory
    {
        AddNew = 0,
        Edit = 1,
        Approve = 2,
        Reject = 3,
        Unapprove = 4,
        Publish = 5,
        Pay = 6,
        Cancel = 7,
        Signed = 8,
        Unsigned = 9
    }
    
    public enum EDocumentStatus
    {
        UnApprove = 0,
        Deny = 1,
        Approve = 2,
        PayFail = 3,
        PaySuccess = 4,
        CancelAppointment = 5,
        Appointment = 6,
        Sign = 7,
        CancelDocument = 8
    }
    

    public enum EStatusAppointmentDate
    {
        Normal = 1,
        Warning = 2,
        Late = 3
    }
    public enum EStatusSign
    {
        All = 1,
        Incomplete = 2,
        Complete = 3
    }

    public enum ERewardStatus
    {
        Waiting = 0, // chờ gửi
        Send = 1, // đã gửi
        Approved = 2, // đã duyệt
        OCR = 3 // đã OCR
    }

    public enum EStatusPay
    {
        Unpaid = 0,
        Sucess = 1,
        Running = 2,
        Error = 3,
    }

    public enum EStatusAnswerQuestion
    {
        Normal = 1,
        Warning = 2,
        Late = 3
    }
    public enum ENotifyType
    {
        Meeting = 0,
        Post = 1,
        SocialNetwork = 2,
        ListSendSendAll = 3,
        ListSendCancelSendAll = 4,
        ListApproveRejectAll = 5,
        ListApproveAcceptAll = 6,
        ListApproveCancelApproveAll = 7
    }

    public enum ETypeImage
    {
        Image = 1,
        Header = 2,
        Footer = 3
    }

    public enum EMeetingMemberType
    {
        Admin = 0,
        Member = 1,
        Waiting = 2
    }

    public enum EMeetingCommentType
    {
        Text = 0,
        Image = 1,
        Vote = 2,
        Election = 3,
        RegistSpeak = 4,
        Note = 5,
        Meeting = 6,
        RegistSpeakView = 7,
        Remind = 8,
        Rating = 9,
        Emotion = 10
    }

    public enum EMeetingVoteSelection
    {
        No = 0,
        Yes = 1,
        Other = 2,
        None = 3
    }

    public enum ECommentStatus
    {
        Lock = 0,
        UnLock = 1,
    }

    public enum ESeatMapType
    {
        Table = 0,//Hội trường
    }

    public enum EMeetingElectionType
    {
        StrikeThrough,// Bầu cử có số dư
        YesNo, // Bầu cử không có số dư
    }

    public enum ESeatNumType
    {
        CT,
        TK,
        BTT,
        DB
    }

    public enum EMeetingVoteType
    {
        OnlyOne = 0,
        Multi = 1
    }

    public enum EWelcomeType
    {
        WC_Frame_Img = 0,
        WC_Logo = 1,
        WC_Title_Welcome = 2,
        WC_Title_AccountName = 3,
        WC_Value_AccountName = 4,
        WC_Value_PositionName = 5,
        WC_Title_Attend = 6,
        WC_TItle_MeetingMember = 7,
        WC_Title_MeetingUnit = 8,
    }

    public enum ERewardSignStatus
    {
        None = 0,
        NotSigned = 1,
        Signed = 2,
        Warning = 3
    }

    public enum ERewardRegistStatus
    {
        Sended = 0,
        Rejected = 1,
        Voted = 2,
        Failed = 3,
        Passed = 4
    }

    public enum ERewardConfigLimitType
    {
        Per = 0,
        Num = 1
    }

    public enum ERewardResolvedType
    {
        RewardName = 0,
        RewardMethod = 1
    }

}
