using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Data;

namespace MASTERPM.Model.Commons
{
    public class MASTERResources
    {
        public static class ID
        {
            public const string _MsgEmailFormat = "_MsgEmailFormat";
            public const string MsgAccessDenined = "MsgAccessDenined";
            public const string MsgSaveOk = "MsgSaveOk";
            public const string MsgUploadOk = "MsgUploadOk";
            public const string MsgDeleteOk = "MsgDeleteOk";
            public const string MsgRejectOk = "MsgRejectOk";
            public const string MsgGetDataOk = "MsgGetDataOk";
            public const string MsgLoginFail = "MsgLoginFail";
            public const string MsgUserLock = "MsgUserLock";
            public const string MsgNotFound = "MsgNotFound";
            public const string MsgNotFoundObject = "MsgNotFoundObject";
            public const string MsgRoleDefault = "MsgRoleDefault";
            public const string MsgException = "MsgException";
            public const string MsgInValidData = "MsgInValidData";
            public const string MsgNotFoundSetting = "MsgNotFoundSetting";
            public const string MsgErrorMaximumLength = "MsgErrorMaximumLength";
            public const string MsgErrorMinimumLength = "MsgErrorMinimumLength";
            public const string MsgErrorMaximumValue = "MsgErrorMaximumValue";
            public const string MsgErrorMinimumValue = "MsgErrorMinimumValue";
            public const string MsgErrorSpecialChar = "MsgErrorSpecialChar";
            public const string MsgErrorRequire = "MsgErrorRequire";
            public const string MsgErrorInvalidType = "MsgErrorInvalidType";
            public const string MsgErrorRegularExpressionNumber = "MsgErrorRegularExpressionNumber";
            public const string MsgErrorRegularExpressionInteger = "MsgErrorRegularExpressionInteger";
            public const string MsgErrorPhone = "MsgErrorPhone";
            public const string MsgErrorDateWithFormat = "MsgErrorDateWithFormat";
            public const string MsgErrorRang = "MsgErrorRang";
            public const string MsgErrorStartEndDatetime = "MsgErrorStartEndDatetime";
            public const string MsgErrorDueDatetime = "MsgErrorDueDatetime";
            public const string MsgErrorVideoUrlMaxSize = "MsgErrorVideoUrlMaxSize";
            public const string MsgErrorFileUrlMaxSize = "MsgErrorFileUrlMaxSize";
            public const string MsgErrorImageUrlMaxSize = "MsgErrorImageUrlMaxSize";
            public const string MsgErrorNotFoundPath = "MsgErrorNotFoundPath";
            public const string MsgErrorBase64Incorrect = "MsgErrorBase64Incorrect";
            public const string MsgErrorRequiredDeleteID = "MsgErrorRequiredDeleteID";
            public const string MsgErrorIsExists = "MsgErrorIsExists";
            public const string MsgErrorIsIDUnitExists = "MsgErrorIsIDUnitExists";
            public const string MsgErrorSignatureIncorrect = "MsgErrorSignatureIncorrect";
            public const string MsgErrorNotFoundConfig = "MsgErrorNotFoundConfig";
            public const string MsgErrorPaymentExisted = "MsgErrorPaymentExisted";
            public const string MsgErrorPaymentNotExpiry = "MsgErrorPaymentNotExpiry";
            public const string MsgErrorHasSpace = "MsgErrorHasSpace";
            public const string MsgErrorPaymentProcess = "MsgErrorPaymentProcess";
            public const string MsgErrorHasException = "MsgErrorHasException";
            public const string MsgPaymentSuccess = "MsgPaymentSuccess";
            public const string MsgPaymentNotSuccess = "MsgPaymentNotSuccess";
            public const string MsgErrorPaymentNotExist = "MsgErrorPaymentNotExist";
            public const string MsgErrorDontInputPhone = "MsgErrorDontInputPhone";
            public const string MsgErrorIDIsNotExists = "MsgErrorIDIsNotExists";
            public const string MsgErrorLogHourOverLog = "MsgErrorLogHourOverLog";
            public const string MsgErrorSearchMonthInvalid = "MsgErrorSearchMonthInvalid";
            public const string MsgErrorSearchPeriodInvalid = "MsgErrorSearchPeriodInvalid";
            public const string MsgErrorPermission = "MsgErrorPermission";
            public const string MsgNoRecordBranch = "MsgNoRecordBranch";
            public const string MsgCreateDataSuccess = "MsgCreateDataSuccess";
            public const string MsgUpdateDataSuccess = "MsgUpdateDataSuccess";
            public const string MsgSuccessPayment = "MsgSuccessPayment";
            public const string MsgApproveDataSuccess = "MsgApproveDataSuccess";
            public const string MsgRejectDataSuccess = "MsgRejectDataSuccess";
            public const string MsgLockDataSuccess = "MsgLockDataSuccess";
            public const string MsgUnLockDataSuccess = "MsgUnLockDataSuccess";
            public const string MsgDeleteDataSuccess = "MsgDeleteDataSuccess";
            public const string MsgUploadDataSuccess = "MsgUploadDataSuccess";
            public const string MsgResolveDataSuccess = "MsgResolveDataSuccess";
            public const string MsgRatingDataSuccess = "MsgRatingDataSuccess";
            public const string MsgLeaveSuccess = "MsgLeaveSuccess";
            public const string MsgAssignUserSuccess = "MsgAssignUserSuccess";
            public const string MsgSendNotifySuccess = "MsgSendNotifySuccess";
            public const string MsgSendMailSuccess = "MsgSendMailSuccess";
            public const string MsgErrorSendMail = "MsgErrorSendMail";
            public const string MsgErrorAccountNotFound = "MsgErrorAccountNotFound";
            public const string MsgErrorUnitNotFound = "MsgErrorUnitNotFound";
            public const string MsgErrorQuestionDup = "MsgErrorQuestionDup";
            public const string MsgError_Negative = "MsgError_Negative";
            public const string MsgErrorInconsonant = "MsgErrorInconsonant";
            public const string MsgErrorStatusInvalid = "MsgErrorStatusInvalid";
            public const string MsgNoData = "MsgNoData";

            public const string MsgValidData = "MsgValidData";
            public const string NotFoundByName = "NotFoundByName";
            public const string Login_IDNotNull = "Login_IDNotNull";
            public const string Login_PhoneNotNull = "Login_PhoneNotNull";
            public const string Login_OTPNotNull = "Login_OTPNotNull";
            public const string Login_OTPOutOfSize = "Login_OTPOutOfSize";
            public const string Login_Phone = "Login_Phone";
            public const string Login_OTPNotMatch = "Login_OTPNotMatch";
            public const string Login_DisplayGender = "Login_DisplayGender_{0}";
            public const string Login_AccLock = "Login_AccLock";
            public const string Login_PhoneMinLength = "Login_PhoneMinLength";
            public const string Login_OTPCodeContent = "Login_OTPCodeContent";
            public const string MsgErrorHaveChildDelete = "MsgErrorHaveChildDelete";
            public const string MsgErrorUserRoleDelete = "MsgErrorUserRoleDelete";
            public const string MsgErrorUserDeleteIssue = "MsgErrorUserDeleteIssue";
            public const string MsgErrorUserDeleteUnit = "MsgErrorUserDeleteUnit";
            public const string MsgErrorPartDeleteUser = "MsgErrorPartDeleteUser";
            public const string MsgErrorSmallerThan = "MsgErrorSmallerThan";
            public const string MsgErrorUnitDeleteUser = "MsgErrorUnitDeleteUser";
            public const string MsgErrorPeriodDelete = "MsgErrorPeriodDelete";
            public const string MsgErrorIssueDeleteType = "MsgErrorIssueDeleteType";
            public const string MsgErrorLogDatetime = "MsgErrorLogDatetime";
            public const string MsgErrorSamePerson = "MsgErrorSamePerson";
            public const string MsgErrorWrongOldPassword = "MsgErrorWrongOldPassword";
            public const string MsgErrorMultiTopic = "MsgErrorMultiTopic";
            public const string MsgErrorNoActiveTopic = "MsgErrorNoActiveTopic";
            public const string MsgErrorNoQuestion = "MsgErrorNoQuestion";
            public const string MsgErrorNoChoice = "MsgErrorNoChoice";
            public const string MsgErrorQuestionSameOrder = "MsgErrorQuestionSameOrder";
            public const string MsgErrorChoiceSameOrder = "MsgErrorChoiceSameOrder";
            public const string MsgErrorProcessSameOrder = "MsgErrorProcessSameOrder";
            public const string MsgErrorProcessSameStartAt = "MsgErrorProcessSameStartAt";
            public const string MsgErrorProcessNegative = "MsgErrorProcessNegative";
            public const string MsgErrorDocumentSameOrder = "MsgErrorDocumentSameOrder";
            public const string MsgErrorDocumentNegative = "MsgErrorDocumentNegative";
            public const string MsgErrorQuestionNullOrder = "MsgErrorQuestionNullOrder";
            public const string MsgErrorChoiceNullOrder = "MsgErrorChoiceNullOrder";
            public const string MsgErrorNoAdmin = "MsgErrorNoAdmin";
            public const string MsgErrorNotPermision = "MsgErrorNotPermision";
            public const string MsgErrorDuplicateMetting = "MsgErrorDuplicateMetting";
            public const string MsgErrorException = "MsgErrorException";
            public const string MsgTitleNotPermision = "MsgTitleNotPermision";
            public const string MsgErrorExistingDataLink = "MsgErrorExistingDataLink";
            public const string MsgTitleDuplicate = "MsgTitleDuplicate";
            public const string MsgTitleSuccessJoinMeeting = "MsgTitleSuccessJoinMeeting";
            public const string MsgAccountPhoneErrorFormat = "MsgAccountPhoneErrorFormat";
            public const string MsgNotPermissionScan = "MsgNotPermissionScan";

            public const string MsgMeetingNotChangeable = "MsgMeetingNotChangeable";

            public const string MsgErrorUpdateMeetingDate = "MsgErrorUpdateMeetingDate";
            public const string MsgErrorUpdateStartAt = "MsgErrorUpdateStartAt";
            public const string MsgErrorUpdateStartAtCurrTime = "MsgErrorUpdateStartAtCurrTime";
            public const string MsgErrorUpdateEndAt = "MsgErrorUpdateEndAt";

            public const string MsgErrorTooManyUnit = "MsgErrorTooManyUnit";

            public const string MsgErrorAccDeleteUser = "MsgErrorAccDeleteUser";

            public const string LabelFormatDate = "";

            public const string MsgErrorInvalidWorkHour = "MsgErrorInvalidWorkHour";
            public const string MsgCheckInOK = "MsgCheckInOK";
            public const string MsgNoMeeting = "MsgNoMeeting";
            public const string MsgCheckInRequired = "MsgCheckInRequired";
            public const string MsgPhoneNotFound = "MsgPhoneNotFound";
            public const string MsgVotingNotStart = "MsgVotingNotStart";
            public const string MsgVotingEnd = "MsgVotingEnd";
            public const string MsgErrorNotPermisionFunc = "MsgErrorNotPermisionFunc";
            public const string MsgErrorOtherProcessApprove = "MsgErrorOtherProcessApprove";
            public const string MsgErrorOtherAccountApproved = "MsgErrorOtherAccountApproved";
            public const string MsgExpressIdea = "MsgExpressIdea";
            public const string MsgVotingNotDeclare = "MsgVotingNotDeclare";
            public const string MsgVotingStarted = "MsgVotingStarted";
            public const string MsgErrorOtherProcessClose = "MsgErrorOtherProcessClose";
            public const string MsgSendRegistSuccess = "MsgSendRegistSuccess";
            public const string MsgDeleteRegistSuccess = "MsgDeleteRegistSuccess";
            public const string MsgWaitingAdminStartMeeting = "MsgWaitingAdminStartMeeting";
            public const string MsgErrorProgressMeettingIsAcctive = "MsgErrorProgressMeettingIsAcctive";
            public const string MsgApprovedMeeting = "MsgApprovedMeeting";
            public const string MsgNotDeleteByApproved = "MsgNotDeleteByApproved";
            public const string MsgNextExpressIdea = "MsgNextExpressIdea";
            public const string MsgStartMeetingSuccess = "MsgStartMeetingSuccess";
            public const string MsgEndMeetingSuccess = "MsgEndMeetingSuccess";
            public const string MsgErrorDeleteMeetingAdmin = "MsgErrorDeleteMeetingAdmin";
            public const string MsgErrorAddressDeleteAddress = "MsgErrorAddressDeleteAddress";
            public const string MsgErrorUpdateMeetingAdmin = "MsgErrorUpdateMeetingAdmin";
            public const string MsgErrorIsExistVotingMember = "MsgErrorIsExistVotingMember";
            public const string MsgErrorMinimumMember = "MsgErrorMinimumMember";
            public const string MsgMeetingEnded = "MsgMeetingEnded";
            public const string MsgQRCodeInvalid = "MsgQRCodeInvalid";
            public const string MsgMeetingEndedAfterTime = "MsgMeetingEndedAfterTime";
            public const string MsgLoginByMobileRequired = "MsgLoginByMobileRequired";
            public const string MsgNotPermissionStartMeeting = "MsgNotPermissionStartMeeting";
            public const string MsgUnDeleteDataSuccess = "MsgUnDeleteDataSuccess";
            public const string MsgDateInValidData = "MsgDateInValidData";
            public const string MsgNotifyMeetingDay = "MsgNotifyMeetingDay";
            public const string MsgOfficeProcessMain = "MsgOfficeProcessMain";
            public const string MsgErrorDateNotValue = "MsgErrorDateNotValue";
            public const string MsgErrorInvalidBirthDate = "MsgErrorInvalidBirthDate";
            public const string MsgErrorSignDateError = "MsgErrorSignDateError";
            public const string MsgErrorSignedError = "MsgErrorSignedError";
            public const string MsgErrorUnSignedError = "MsgErrorUnSignedError";
            public const string MsgErrorMissingContactDB = "MsgErrorMissingContactDB";

            public const string MsgErrorInvalidCVPro5Group = "MsgErrorInvalidCVPro5Group";

            //Office
            public const string Label_Office_IDList = "Label_Office_IDList";
            public const string Label_Office = "Label_Office";
            public const string Label_Office_ID = "Label_Office_ID";
            public const string Label_Office_Name = "Label_Office_Name";
            public const string Label_Office_Description = "Label_Office_Description";
            public const string Label_Office_SerialNo = "Label_Office_SerialNo";
            public const string Label_Office_OfficeDt = "Label_Office_OfficeDt";
            public const string Label_Office_TypeID = "Label_Office_TypeID";
            public const string Label_Office_EndDt = "Label_Office_EndDt";


            //title page
            public const string Label_page_ID = "Label_page_ID";
            public const string Label_page_Nm = "Label_page_Nm";
            public const string Label_obj_page = "Label_obj_page";

            //User
            public const string User_UserName = "User_UserName";
            public const string User_Password = "User_Password";
            public const string User_Description = "User_Description";
            public const string User_RoleID = "User_RoleID";
            public const string User_FullName = "User_FullName";
            public const string User_UnitID = "User_UnitID";
            public const string User_PartID = "User_PartID";
            public const string User_Phone = "User_Phone";
            public const string User_Email = "User_Email";
            public const string User_Account_Info = "User_Account_Info";

            //Control
            public const string Control_ID = "Control_ID";
            public const string Control_Name = "Control_Name";

            //branch
            public const string Label_Branch = "Label_Branch";

            //deal
            public const string Label_Deal = "Label_Deal";

            //account
            public const string Txt_AccountNm = "Txt_AccountNm";

            //Role
            public const string Label_Role_ID = "Label_Role_ID";
            public const string Label_Role_Name = "Label_Role_Name";
            //Pro5
            public const string Label_Pro5_ID = "Label_Pro5_ID";
            public const string Label_Pro5_Name = "Label_Pro5_Name";

            //Part
            public const string Label_Part_ID = "Label_Part_ID";
            public const string Label_Part_Name = "Label_Part_Name";
            public const string Label_Part_Description = "Label_Part_Description";

            //IssueType
            public const string Label_IssueType_ID = "Label_IssueType_ID";
            public const string Label_IssueType_Name = "Label_IssueType_Name";

            public const string Label_Archive_Name = "Label_Archive_Name";
            public const string Label_Archive_File_Name = "Label_Archive_File_Name";

            //RolePage
            public const string Label_RolePage_RoleID = "Label_RolePage_RoleID";
            public const string Label_RolePage_PageID = "Label_RolePage_PageID";
            public const string Label_RolePage_ControlStr = "Label_RolePage_ControlStr";

            //Project 
            public const string Label_Project_ID = "Label_Project_ID";
            public const string Label_Project_Name = "Label_Project_Name";
            public const string Label_Project_Code = "Label_Project_Code";

            //Unit 
            public const string Label_Unit_ID = "Label_Unit_ID";
            public const string Label_Unit_Name = "Label_Unit_Name";
            public const string Label_Unit_ParentID = "Label_Unit_ParentID";
            public const string Label_Unit_Leader = "Label_Unit_Leader";
            public const string Label_Unit_Level = "Label_Unit_Level";
            public const string Label_Level = "Label_Level";
            //Issue 
            public const string Label_Issue_ID = "Label_Issue_ID";
            public const string Label_Issue_Code = "Label_Issue_Code";
            public const string Label_Issue_ProjectID = "Label_Issue_ProjectID";
            public const string Label_Issue_TypeID = "Label_Issue_TypeID";
            public const string Label_Issue_Summary = "Label_Issue_Summary";
            public const string Label_Issue_Description = "Label_Issue_Description";
            public const string Label_Issue_Priority = "Label_Issue_Priority";
            public const string Label_Issue_StartDate = "Label_Issue_StartDate";
            public const string Label_Issue_DueDate = "Label_Issue_DueDate";
            public const string Label_Issue_Status = "Label_Issue_Status";
            public const string Label_Issue_ParentID = "Label_Issue_ParentID";
            public const string Label_Issue_RelatedPersonel = "Label_Issue_RelatedPersonel";
            public const string Label_Issue_LinkType = "Label_Issue_LinkType";
            public const string Label_Issue_LinkID = "Label_Issue_LinkID";
            public const string Label_Issue_LinkUrl = "Label_Issue_LinkUrl";
            public const string Label_Issue_ProgressID = "Label_Issue_ProgressID";
            public const string Label_Issue_Reporter = "Label_Issue_Reporter";
            public const string Label_Issue_Assignee = "Label_Issue_Assignee";
            public const string Label_Issue_AssigneeFrom = "Label_Issue_AssigneeFrom";
            public const string MsgRestartIssueOk = "MsgRestartIssueOk";

            //Comment
            public const string Label_Comment_ID = "Label_Comment_ID";
            public const string Label_Comment_IssueID = "Label_Comment_IssueID";
            public const string Label_Comment_Reporter = "Label_Comment_Reporter";
            public const string Label_Comment_Description = "Label_Comment_Description";
            public const string Label_Comment_FileUrls = "Label_Comment_FileUrls";

            //LogWork
            public const string Label_LogWork_ID = "Label_LogWork_ID";
            public const string Label_LogWork_IssueID = "Label_LogWork_IssueID";
            public const string Label_LogWork_Reporter = "Label_LogWork_Reporter";
            public const string Label_LogWork_LogDate = "Label_LogWork_LogDate";
            public const string Label_LogWork_Worked = "Label_LogWork_Worked";
            public const string Label_LogWork_Description = "Label_LogWork_Description";

            // File
            public const string Label_File_Code = "Label_File_Code";
            public const string Label_File_SerialNo = "Label_File_SerialNo";
            public const string Label_File_ID = "Label_File_ID";
            public const string Label_File_Title = "Label_File_Title";
            public const string Label_File_Description = "Label_File_Description";
            public const string Label_File_UserName = "Label_File_UserName";
            public const string Label_File_AssignUnits = "Label_File_AssignUnits";
            public const string Label_File_FileUrls = "Label_File_FileUrls";
            public const string Label_File_DelFlg = "Label_File_DelFlg";
            public const string Label_File_CreateAt = "Label_File_CreateAt";
            public const string Label_File_UpdateAt = "Label_File_UpdateAt";

            // FileLog
            public const string Label_FileLog_ID = "Label_FileLog_ID";
            public const string Label_FileLog_UserName = "Label_FileLog_UserName";
            public const string Label_FileLog_CreateAt = "Label_FileLog_CreateAt";

            // GroupChat
            public const string Label_GroupChat_ID = "Label_GroupChat_ID";
            public const string Label_GroupChat_Name = "Label_GroupChat_Name";
            public const string Label_GroupChat_Member = "Label_GroupChat_Member";

            //report

            public const string Label_Report_Unexpired = "Label_Report_Unexpired";
            public const string Label_Report_Expired = "Label_Report_Expired";
            public const string Label_Report_LeaderinMonth = "Label_Report_LeaderinMonth";
            public const string Label_Report_LeaderinNextMonth = "Label_Report_LeaderinNextMonth";
            public const string Label_Report_CompleteinMonth = "Label_Report_CompleteinMonth";
            public const string Label_Report_StillValid = "Label_Report_StillValid";
            public const string Label_Report_OutDate1 = "Label_Report_OutDate1";
            public const string Label_Report_OnTime = "Label_Report_OnTime";
            public const string Label_Report_OutDate2 = "Label_Report_OutDate2";
            public const string Label_Report_ApprovedComplete = "Label_Report_ApprovedComplete";
            public const string Label_Report_NotComplete = "Label_Report_NotComplete";
            public const string Label_Report_NotRating = "Label_Report_NotRating";
            public const string Label_Report_Sum = "Label_Report_Sum";
            public const string Label_Report_SumOut = "Label_Report_SumOut";

            // meeting 
            public const string Label_Meeting_Admin = "Label_Meeting_Admin";
            public const string Label_Meeting_Date = "Label_Meeting_Date";
            public const string Label_Meeting_StartAt = "Label_Meeting_StartAt";
            public const string Label_Meeting_ID = "Label_Meeting_ID";
            public const string Label_Meeting_EndAt = "Label_Meeting_EndAt";
            public const string Label_Meeting_Name = "Label_Meeting_Name";
            public const string Label_Meeting_Address = "Label_Meeting_Address";
            public const string Label_Meeting_Element = "Label_Meeting_Element";
            public const string Label_Meeting_EndDate = "Label_Meeting_EndDate";
            public const string Label_Meeting_FromDate = "Label_Meeting_FromDate";
            public const string Label_Meeting_ToDate = "Label_Meeting_ToDate";
            public const string Label_Meeting_Note = "Label_Meeting_Note";
            public const string Label_Meeting_Guest = "Label_Meeting_Guest";


            // member 
            public const string Label_Member_Seat_Position = "Label_Member_Seat_Position";


            // event 
            public const string Label_Event_ID = "Label_Event_ID";
            public const string Label_Event_Name = "Label_Event_Name";
            public const string Label_Event_Admin = "Label_Event_Admin";
            public const string Label_Event_Date = "Label_Event_Date";
            public const string Label_Event_StartAt = "Label_Event_StartAt";
            public const string Label_Event_EndAt = "Label_Event_EndAt";
            public const string Label_Event_Tag = "Label_Event_Tag";
            public const string Label_Event_Type = "Label_Event_Type";

            public const string MsgErrorUpdateEventDate = "MsgErrorUpdateEventDate";
            public const string MsgErrorUpdateEventStartAt = "MsgErrorUpdateEventStartAt";
            public const string MsgErrorUpdateEventEndAt = "MsgErrorUpdateEventEndAt";
            public const string MsgErrorEventDueDatetime = "MsgErrorEventDueDatetime";
            public const string MsgErrorUpdateMeetingEndDate = "MsgErrorUpdateMeetingEndDate";


            public const string Msg_Error_Meeting_Ongoing = "Msg_Error_Meeting_Ongoing";
            public const string Msg_Error_Meeting_InputFlg = "Msg_Error_Meeting_InputFlg";

            //question
            public const string Label_Question_StartAt = "Label_Question_StartAt";
            public const string Label_Question_ID = "Label_Question_ID";
            public const string Label_Question_EndAt = "Label_Question_EndAt";
            public const string Label_Question_Name = "Label_Question_Name";
            public const string Label_Question_ParentID = "Label_Question_ParentID";
            public const string Label_Question_GroupID = "Label_Question_GroupID";
            public const string Label_Question_Type = "Label_Question_Type";
            public const string Label_Question_NumberSelection = "Label_Question_NumberSelection";

            public const string MsgError_Question_NumberSelection = "MsgError_Question_NumberSelection";

            //Process
            public const string Label_Process_Name = "Label_Process_Name";
            public const string Label_Process_StartAt = "Label_Process_StartAt";

            //data-account
            public const string Label_DataAcc_PersonalID = "Label_DataAcc_PersonalID";
            public const string Label_DataAcc_UnitID = "Label_DataAcc_UnitID";
            public const string Label_MeetingID = "Label_MeetingID";
            public const string Label_AssignList = "Label_AssignList";

            //idea
            public const string Label_Idea_ID = "Label_Idea_ID";
            public const string Label_IdeaDetail_ID = "Label_IdeaDetail_ID";

            //Reward
            public const string Label_Reward_PersonalID = "Label_Reward_PersonalID";
            public const string Label_Reward_Date = "Label_Reward_Date";
            public const string Label_Reward_Contents = "Label_Reward_Contents";
            public const string Label_Reward_Unit = "Label_Reward_Unit";

            //Address
            public const string Label_Address_ID = "Label_Address_ID";
            public const string Label_Address_Name = "Label_Address_Name";
            public const string Label_Address_Type = "Label_Address_Type";

            //Position
            public const string Label_Position_ID = "Label_Position_ID";
            public const string Label_Position_Name = "Label_Position_Name";

            //Study
            public const string Label_Study_FromDate = "Label_Study_FromDate";
            public const string Label_Study_ToDate = "Label_Study_ToDate";
            public const string Label_Study_SchoolName = "Label_Study_SchoolName";
            public const string Label_Study_Method = "Label_Study_Method";

            //Work        
            public const string Label_Work_Job = "Label_Work_Job";
            public const string Label_Work_Place = "Label_Work_Place";
            public const string Label_Work_Position = "Label_Work_Position";

            //Config
            public const string Label_Config_ID = "Label_Config_ID";
            public const string Label_Config_Value = "Label_Config_Value";
            public const string Label_Config_Description = "Label_Config_Description";

            //Common
            public const string Common_Name = "Common_Name";
            public const string Common_Type = "Common_Type";

            //Family
            public const string Family_Name = "Family_Name";
            public const string Family_Relation = "Family_Relation";
            public const string Family_BirthDate = "Family_BirthDate";
            public const string Family_HomeTown = "Family_HomeTown";
            public const string Family_Address = "Family_Address";
            public const string Family_Job = "Family_Job";
            //Abroad
            public const string Label_Abroad_Reason = "Label_Abroad_Reason";
            public const string Label_Abroad_Place = "Label_Abroad_Place";

            //Asset
            public const string Label_Asset_Type = "Label_Asset_Type";
            public const string Label_Asset_Quantity = "Label_Asset_Quantity";
            public const string Label_Asset_Value = "Label_Asset_Value";
            public const string Label_Asset_Reason = "Label_Asset_Reason";

            //Personal

            public const string Label_Personal_ID = "Label_Personal_ID";
            public const string Label_Personal_CurrentName = "Label_Personal_CurrentName";
            public const string Label_Personal_BirthDate = "Label_Personal_BirthDate";
            public const string Label_Personal_DTNJoinDate = "Label_Personal_DTNJoinDate";
            public const string Label_Personal_DeathDate = "Label_Personal_DeathDate";
            public const string Label_Personal_PartyOfficialDate = "Label_Personal_PartyOfficialDate";
            public const string Label_Personal_Gender = "Label_Personal_Gender";
            public const string Label_Personal_HomeTown = "Label_Personal_HomeTown";
            public const string Label_Personal_PartyJoinDate = "Label_Personal_PartyJoinDate";
            public const string Label_Personal_PartyJoinPlaceID = "Label_Personal_PartyJoinPlaceID";
            public const string Label_Personal_PartyMemberType = "Label_Personal_PartyMemberType";
            public const string Label_Personal_PartyCardID = "Label_Personal_PartyCardID";
            public const string Label_Personal_PartyProfileNo = "Label_Personal_PartyProfileNo";
            public const string Label_Personal_BirthName = "Label_Personal_BirthName";
            public const string Label_Personal_Alias = "Label_Personal_Alias";
            public const string Label_Personal_HKTT = "Label_Personal_HKTT";
            public const string Label_Personal_CurrentAddress = "Label_Personal_CurrentAddress";
            public const string Label_Personal_Phone = "Label_Personal_Phone";
            public const string Label_Personal_CurrentJob = "Label_Personal_CurrentJob";
            public const string Label_Personal_DTNJoinPlaceID = "Label_Personal_DTNJoinPlaceID";
            public const string Label_Personal_PartyOfficialPlaceID = "Label_Personal_PartyOfficialPlaceID";
            public const string Label_Personal_Introducer1 = "Label_Personal_Introducer1";
            public const string Label_Personal_Introducer2 = "Label_Personal_Introducer2";
            public const string Label_Personal_Review_File = "Label_Personal_Review_File";

            //Process Comment
            public const string Label_Process_Cmt_Fields = "Label_Process_Cmt_Fields";
            public const string MsgProcessSMS = "MsgProcessSMS";
            public const string User_Unit = "User_Unit";
            public const string MsgEndDateProcess = "MsgEndDateProcess";
            public const string MsgFeedbackDateProcess = "MsgFeedbackDateProcess";
            public const string MsgErrorIsExistsArchiveFolder = "MsgErrorIsExistsArchiveFolder";

            //Personal
            public const string MsgPersonalUnitExist = "MsgPersonalUnitExist";

            //Row
            public const string RowName = "RowName";
            public const string ColumnName = "ColumnName";
            public const string ColumnWidth = "ColumnWidth";

            //Period Report
            public const string Period_Name = "Period_Name";
            public const string Period_Strart = "Period_Strart";
            public const string Period_End = "Period_End";
            public const string Period_Parent = "Period_Parent";
            public const string Period_Null_Time = "Period_Null_Time";
            public const string Period_Error_Time = "Period_Error_Time";
            public const string Period_Template = "Period_Template";

            //Newsfeed
            public const string Label_News_Description = "Label_News_Description";
            public const string Label_News_Media = "Label_News_Media";
            public const string MsgErrorMediaTooMany = "MsgErrorMediaTooMany";

            public const string MsgVnpayLockOrderIdNotFound = "MsgVnpayLockOrderIdNotFound";
            public const string MsgVnpayLockStatusNotSuccess = "MsgVnpayLockStatusNotSuccess";
            public const string MsgVnpayLockAmountNotMatch = "MsgVnpayLockAmountNotMatch";
            public const string MsgVnpayPaid = "MsgVnpayPaid";



            //Unit Contact
            public const string Label_Unit_Contact_Name = "Label_Unit_Contact_Name";
            public const string Label_Unit_Contact_Address = "Label_Unit_Contact_Address";
            public const string Label_Unit_Contact_Description = "Label_Unit_Contact_Description";

            public const string Label_Unit_Contact_Name_En_Lan = "Label_Unit_Contact_Name_En_Lan";
            public const string Label_Unit_Contact_Address_En_Lan = "Label_Unit_Contact_Address_En_Lan";
            public const string Label_Unit_Contact_Description_En_Lan = "Label_Unit_Contact_Description_En_Lan";


            public const string _MsgRequired = "_MsgRequired";
            public const string _MsgDateLessThanError = "_MsgDateLessThanError";
            public const string _MsgDateEOGThanError = "_MsgDateEOGThanError";
            public const string _MsgMaxLength20 = "_MsgMaxLength20";
            public const string _MsgMaxLength150 = "_MsgMaxLength150";
            public const string _MsgMaxLength100 = "_MsgMaxLength100";
            public const string _MsgRequiredNumber = "_MsgRequiredNumber";
            public const string _MsgNotUpdated = "_MsgNotUpdated";
            public const string _MsgNotValidState = "_MsgNotValidState";
            public const string _MsgMaxLength200 = "_MsgMaxLength200";
            public const string _MsgMaxLength500 = "_MsgMaxLength500";
            public const string _MsgRangeValue_0_32767 = "_MsgRangeValue_0_32767";



            //ReportTemplate
            public const string RpTemplate_ID = "RpTemplate_ID";
            public const string RpTemplate_Name = "RpTemplate_Name";
            public const string RpTemplate_Des = "RpTemplate_Des";
            public const string RpTemplate_Index = "RpTemplate_Index";

            //DynamicReport
            public const string DynamicReport_Value = "DynamicReport_Value";
            
            //ProfileStep
            public const string Pro5Step_Name = "Pro5Step_Name";
            public const string Pro5Step_Description = "Pro5Step_Description";
            public const string Pro5Step_OrderIndex = "Pro5Step_OrderIndex";
            public const string _CV_PRO5_GROUP_STEP = "_CV_PRO5_GROUP_STEP";
            //Payment
            public const string _M_PAYMENT = "_M_PAYMENT";
            public const string Payment_Name = "Payment_Name";
            public const string Payment_Description = "Payment_Description";
            public const string Payment_OrderIndex = "Payment_OrderIndex";
            public const string _MsgMaxLength250 = "_MsgMaxLength250";

            public const string OrderIndex_Common = "OrderIndex_Common";
            public const string MsgErrorUnitDeleteMenu = "MsgErrorUnitDeleteMenu";

            //SMS inform 
            public const string Register_Success = "Register_Success";
            public const string Register_SeCode = "Register_SeCode";
            public const string Email_subject = "Email_subject";

            // Mail Approve/Deny
            public const string Notice_Of_Document_Approval = "Notice_Of_Document_Approval";
            public const string Notice_Of_Document_Denying = "Notice_Of_Document_Denying";

            //Notify Document Day
            public const string MsgNotifyDocDayVN = "MsgNotifyDocDayVN";
            public const string MsgNotifyDocDayEN = "MsgNotifyDocDayEN";
            //Notify ReleaseDocument
            public const string MsgNotifyReleaseVN = "MsgNotifyReleaseVN";
            public const string MsgNotifyReleaseEN = "MsgNotifyReleaseEN";

            public const string Msg_AccountLoginNotFound = "Msg_AccountLoginNotFound";
            public const string MsgCommentLock = "MsgCommentLock";
            public const string MsgChatNotInGroup = "MsgChatNotInGroup";
            public const string MsgMeetingNotExists = "MsgMeetingNotExists";
            public const string MsgMeetingIsNotStart = "MsgMeetingIsNotStart";
            public const string MsgMeetingIsEnded = "MsgMeetingIsEnded";
            public const string MsgMeetingVoteIsNotChange = "MsgMeetingVoteIsNotChange";
            public const string MsgMeetingVoteNotFound = "MsgMeetingVoteNotFound";
            public const string MsgMeetingVoteIsNotSend = "MsgMeetingVoteIsNotSend";
            public const string MsgSocialNetworkLockOrDel = "MsgSocialNetworkLockOrDel";
            public const string Label_Menu_Meeting = "Label_Menu_Meeting";
            public const string Label_Menu_Post = "Label_Menu_Post";
            public const string Label_Menu_SocialNetwork = "Label_Menu_SocialNetwork";
            public const string Label_Menu_Map = "Label_Menu_Map";
            public const string Label_Menu_Setting = "Label_Menu_Setting";
            public const string Label_PostComment_Status_Lock = "Label_PostComment_Status_Lock";
            public const string Label_PostComment_Status_UnLock = "Label_PostComment_Status_UnLock";
            public const string MsgCommentUnLock = "MsgCommentUnLock";
            public const string MsgSocialNetworkLock = "MsgSocialNetworkLock";
            public const string Label_PostComment_Status_All = "Label_PostComment_Status_All";
            public const string MsgSocialNetWorkUnLock = "MsgSocialNetWorkUnLock";
            public const string Label_SocialNetwork_Status_UnLock = "Label_SocialNetwork_Status_UnLock";
            public const string Label_SocialNetwork_Status_Lock = "Label_SocialNetwork_Status_Lock";
            public const string MsgMeetingElectionNotFound = "MsgMeetingElectionNotFound";
            public const string MsgMeetingElectionIsNotChange = "MsgMeetingElectionIsNotChange";
            public const string MsgMeetingElectionIsNotSend = "MsgMeetingElectionIsNotSend";
            public const string Label_MeetingElection_None = "Label_MeetingElection_None";
            public const string MsgMeetingVoteNotEnd = "MsgMeetingVoteNotEnd";
            public const string MsgMeetingElectionNotEnd = "MsgMeetingElectionNotEnd";
            public const string Label_ESeatMapType_Table = "Label_ESeatMapType_Table";
            public const string MsgMeetingRegistSpeakNotFound = "MsgMeetingRegistSpeakNotFound";
            public const string MsgMeetingRegistSpeakIsNotChange = "MsgMeetingRegistSpeakIsNotChange";
            public const string Label_MeetingImageBody = "Label_MeetingImageBody";
            public const string Label_MeetingAddMemberBody = "Label_MeetingAddMemberBody";
            public const string Label_MeetingUpdateSeatMemberBody = "Label_MeetingUpdateSeatMemberBody";
            public const string Label_MeetingRemoveMemberBody = "Label_MeetingRemoveMemberBody";
            public const string Label_SocialNetwork_Insert = "Label_SocialNetwork_Insert";
            public const string Label_SocialNetwork_Update = "Label_SocialNetwork_Update";
            public const string Label_SocialNetworkComment_Insert = "Label_SocialNetworkComment_Insert";
            public const string Label_SocialNetworkComment_Update = "Label_SocialNetworkComment_Update";
            public const string Label_SocialNetworkLike_Insert = "Label_SocialNetworkLike_Insert";
            public const string Label_SocialNetworkLike_Delete = "Label_SocialNetworkLike_Delete";
            public const string Label_Post_Status_Approve = "Label_Post_Status_Approve";
            public const string Label_Post_Status_UnApprove = "Label_Post_Status_UnApprove";
            public const string MsgPostApprove = "MsgPostApprove";
            public const string MsgPostUnApprove = "MsgPostUnApprove";
            public const string LabelPostLatest = "LabelPostLatest";
            public const string MsgEmailConfigNotFound = "MsgEmailConfigNotFound";
            public const string Label_ForgotPassword = "Label_ForgotPassword";
            public const string MsgEmailIsExists = "MsgEmailIsExists";
            public const string MsgEmailIsConflict = "MsgEmailIsConflict";
            public const string MsgUpdatePasswordSuccess = "MsgUpdatePasswordSuccess";
            public const string MsgPasswordNotCorrect = "MsgPasswordNotCorrect";
            public const string MsgPasswordNotMatch = "MsgPasswordNotMatch";
            public const string MsgSecureCodeNotValid = "MsgSecureCodeNotValid";
            public const string MsgMeetingMemberNotFound = "MsgMeetingMemberNotFound";
            public const string MsgAccountNotFound = "MsgAccountNotFound";
            public const string MsgSeatMapRowLimit = "MsgSeatMapRowLimit";
            public const string MsgMeetingMemberSeatMapIsExists = "MsgMeetingMemberSeatMapIsExists"; 
            public const string MsgAcceptExtension = "MsgAcceptExtension";
            public const string MsgFileUploadNotValid = "MsgFileUploadNotValid";
            public const string MsgFileUploadNotAccept = "MsgFileUploadNotAccept";
            public const string MsgMeetingElectionIsNotEnd = "MsgMeetingElectionIsNotEnd";
            public const string MsgStrikeThroughNumMax = "MsgStrikeThroughNumMax";
            public const string MsgMeetingCommentCanNotDelete = "MsgMeetingCommentCanNotDelete";
            public const string MsgErrUserNameIsExixts = "MsgErrUserNameIsExixts";
            public const string MsgErrUnitIDIsExixts = "MsgErrUnitIDIsExixts"; 
            public const string MsgErrUnitNameIsExixts = "MsgErrUnitNameIsExixts";
            public const string MsgErrUnitIDNotExixts = "MsgErrUnitIDNotExixts";
            public const string MsgRoleDefaultNotFound = "MsgRoleDefaultNotFound";
            public const string MsgRoleNotFound = "MsgRoleNotFound";
            public const string MsgPositionNotFound = "MsgPositionNotFound";
            public const string MsgUnitNotFound = "MsgUnitNotFound";
            public const string MsgMeetingParentNotExists = "MsgMeetingParentNotExists";
            public const string MsgMeetingMemberParentNotExists = "MsgMeetingMemberParentNotExists";
            public const string MsgMeetingGroupNotFound = "MsgMeetingGroupNotFound";
            public const string Label_UnitType = "Label_UnitType";
            public const string Label_Qualification = "Label_Qualification";
            public const string MsgMeetingCurInGroupIsExists = "MsgMeetingCurInGroupIsExists";

            public const string MsgMeetingMemberRegistSuccess = "MsgMeetingMemberRegistSuccess";
            public const string MsgMeetingMemberUnRegistSuccess = "MsgMeetingMemberUnRegistSuccess";
            public const string MsgMeetingMemberApproveSuccess = "MsgMeetingMemberApproveSuccess";
            public const string MsgMeetingMemberUnApproveSuccess = "MsgMeetingMemberUnApproveSuccess";
            public const string MsgMeetingMemberIsExistsInMeeting = "MsgMeetingMemberIsExistsInMeeting";
            public const string MsgMeetingInGroupIsExists = "MsgMeetingInGroupIsExists";
            public const string MsgUserAccountIsExist = "MsgUserAccountIsExist";
            public const string Label_MeetingElection_Start_Title = "Label_MeetingElection_Start_Title";
            public const string Label_MeetingVote_Start_Title = "Label_MeetingVote_Start_Title";
            public const string Label_MeetingElection_Remind_Description = "Label_MeetingElection_Remind_Description";
            public const string MsgMeetingSeatMapOutOfValue = "MsgMeetingSeatMapOutOfValue"; 
            public const string MsgRegistSpeakDateTimeNotValid = "MsgRegistSpeakDateTimeNotValid";
            public const string MsgMeetingMemberNotSendComment = "MsgMeetingMemberNotSendComment";
            public const string MsgMeetingGroupDuplicateDate = "MsgMeetingGroupDuplicateDate";
            public const string MsgMeetingEndTitle = "MsgMeetingEndTitle";
            public const string MsgMeetingEndDes = "MsgMeetingEndDes";
            public const string MsgMeetingVoteMemberEnd = "MsgMeetingVoteMemberEnd";
            public const string MsgMeetingElectionMemberEnd = "MsgMeetingElectionMemberEnd";
            public const string MsgMeetingRatingEnd = "MsgMeetingRatingEnd";
            public const string Label_MeetingVote_Remind_Description = "Label_MeetingVote_Remind_Description";
            public const string MsgMeetingCommentRating_Description = "MsgMeetingCommentRating_Description";
            public const string MsgMeetingVoteEnd = "MsgMeetingVoteEnd";
            public const string MsgMeetingElectionEnd = "MsgMeetingElectionEnd";
            public const string MsgMeetingVote2 = "MsgMeetingVote2";
            public const string MsgMeetingElection_MemberAdmin = "MsgMeetingElection_MemberAdmin";
            public const string MsgMeetingVote_MemberAdmin = "MsgMeetingVote_MemberAdmin";
            public const string MsgMeeting_MemberAdmin = "MsgMeeting_MemberAdmin";
            public const string MsgMeetingVoteNotEnd_NotPublish = "MsgMeetingVoteNotEnd_NotPublish";
            public const string MsgMeetingElectionNotEnd_NotPublish = "MsgMeetingElectionNotEnd_NotPublish";
            public const string MsgMeetingVote_PublishMemberAdmin = "MsgMeetingVote_PublishMemberAdmin";
            public const string MsgMeetingElection_PublishMemberAdmin = "MsgMeetingElection_PublishMemberAdmin";
            public const string MsgMeetingCommentMeetingEnd_Description = "MsgMeetingCommentMeetingEnd_Description";
            public const string MsgMeetingVote2_NotValid = "MsgMeetingVote2_NotValid";
            public const string MsgMeetingElection_MemberTTBC = "MsgMeetingElection_MemberTTBC";
            public const string MsgMeetingVote_MemberTBBC = "MsgMeetingVote_MemberTBBC";
            public const string MsgMeetingElection_Start_MemberTTBC = "MsgMeetingElection_Start_MemberTTBC";
            public const string MsgMeetingVote_Start_MemberTBBC = "MsgMeetingVote_Start_MemberTBBC";
            public const string MsgAccountImportExcelDuplicate = "MsgAccountImportExcelDuplicate";
            public const string MsgOrganizationImportExcelDuplicate = "MsgOrganizationImportExcelDuplicate";
            public const string MsgMeeting_MemberAdmin_CheckIn = "MsgMeeting_MemberAdmin_CheckIn";
            public const string MsgMeetingCheckinR2Started = "MsgMeetingCheckinR2Started";
            public const string MsgMeetingElectionMemberMin1 = "MsgMeetingElectionMemberMin1";
            public const string MsgMeetingMemberCheckin_Started = "MsgMeetingMemberCheckin_Started";
            public const string MsgMeetingVoteResultNotFound = "MsgMeetingVoteResultNotFound";
            public const string MsgMeetingMemberCheckin_Not = "MsgMeetingMemberCheckin_Not";
            public const string MsgMeetingVoteStarting = "MsgMeetingVoteStarting";
            public const string MsgMeetingElectionStarting = "MsgMeetingElectionStarting";
            public const string MsgMeetingElectionResultNotFound = "MsgMeetingElectionResultNotFound";
            public const string MsgMeeting_MemberConfigViewCheckIn = "MsgMeeting_MemberConfigViewCheckIn";
            public const string Msg_Error_Access_Denied = "Msg_Error_Access_Denied";
            public const string MsgMeetingVote_ExportMemberAdmin = "MsgMeetingVote_ExportMemberAdmin";
            public const string Label_MeetingRemind = "Label_MeetingRemind";
            public const string MsgErrUserNameIsNotExixts = "MsgErrUserNameIsNotExixts";
            public const string MsgSeatNumTypeNotExists = "MsgSeatNumTypeNotExists";
            public const string MsgSeatNumImportExcelDuplicate = "MsgSeatNumImportExcelDuplicate";
            public const string MsgUserNameInHeaderNullOrEmpty = "MsgUserNameInHeaderNullOrEmpty";
            public const string MsgErrorUnitGroupDelete = "MsgErrorUnitGroupDelete";
            public const string MsgPeriodYearDelete = "MsgPeriodYearDelete";

            public const string MsgRewardRegistNotAccount = "MsgRewardRegistNotAccount";
            public const string MsgRewardRegistNotUnit = "MsgRewardRegistNotUnit";
            public const string MsgRewardRegistNotDelete = "MsgRewardRegistNotDelete";
            public const string MsgApprovedOk = "MsgApprovedOk";
            public const string MsgRejectedOk = "MsgRejectedOk";
            public const string MsgPassedOk = "MsgPassedOk";
            public const string MsgFailedOk = "MsgFailedOk";
            public const string MsgErrAccountHaveUserExixts = "MsgErrAccountHaveUserExixts";
            public const string MsgRewardRegistDueDate = "MsgRewardRegistDueDate";
            public const string MsgRewardRegistNotChange = "MsgRewardRegistNotChange";
            public const string MsgPeriodStartEnd = "MsgPeriodStartEnd";
            public const string MsgPeriodBetweenStartEndBetweenPeriodYear = "MsgPeriodBetweenStartEndBetweenPeriodYear";
            public const string MsgPeriodDueBetweenStartEnd = "MsgPeriodDueBetweenStartEnd";
            public const string MsgUnitInUnitType = "MsgUnitInUnitType";
            public const string MsgAccountBirthdayToday = "MsgAccountBirthdayToday";
            public const string MsgRewardPassedLimit = "MsgRewardPassedLimit";
            public const string MsgPeriodYearNameIsExists = "MsgPeriodYearNameIsExists";
            public const string MsgPeriodNameIsExists = "MsgPeriodNameIsExists";
            public const string MsgRewardResolvedCodeIsExists = "MsgRewardResolvedCodeIsExists";
            public const string MsgRewardNameNameIsExists = "MsgRewardNameNameIsExists";
            public const string MsgRewardMethodNameIsExists = "MsgRewardMethodNameIsExists";
            public const string MsgRewardRegistStartDate_EndDate = "MsgRewardRegistStartDate_EndDate";
            public const string MsgPeriodStartEndVotedBetweenPeriod = "MsgPeriodStartEndVotedBetweenPeriod";
            public const string MsgPeriodStartEndVoted = "MsgPeriodStartEndVoted";
            public const string MsgRewardStatusNotNone = "MsgRewardStatusNotNone";
            public const string MsgPdfUrlNotFound = "MsgPdfUrlNotFound";
            public const string MsgErrorRewardNotExist = "MsgErrorRewardNotExist";
            public const string MsgErrorRewardSigned = "MsgErrorRewardSigned"; 
            public const string LabelSigned_Warning = "LabelSigned_Warning";
            public const string Label_Signed = "Label_Signed";
            public const string Label_NotSigned = "Label_NotSigned";
        }

    private static MASTERResources instance;
        public static MASTERResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MASTERResources();
                }

                return instance;
            }
        }

        public string Get(ModelStateDictionary modelState)
        {
            var results = new List<string>();
            var msg = "";
            foreach (ModelState model in modelState.Values)
            {
                foreach (ModelError error in model.Errors)
                {
                    msg = error.ErrorMessage;
                    if (string.IsNullOrEmpty(msg) && error.Exception != null && !string.IsNullOrEmpty(error.Exception.Message))
                    {
                        msg = error.Exception.Message;
                    }
                    results.Add(msg);
                }
            }

            return string.Join(";", results);
        }

        public string Get(string key)
        {
            return GlobalResources.Resources.ResourceManager.GetString(key);
        }

        public DataTable GetDataTable(ModelStateDictionary modelState)
        {
            var keys = modelState.Keys.AsEnumerable().Select(r => r.Trim()).ToList();
            var values = modelState.Values.AsEnumerable().Select(r => r.Errors).ToList();

            var results = new DataTable();
            var dataRow = results.NewRow();
            var isErr = false;
            for (int i = 0; i < keys.Count; i++)
            {
                var idx = keys[i].IndexOf(".");
                var colName = idx >= 0 ? keys[i].Substring(idx + 1) : keys[i];

                if (!results.Columns.Contains(colName)) {
                    results.Columns.Add(new DataColumn(colName));
                    dataRow[colName] = string.Join("; ", values[i].AsEnumerable()
                        .Select(r => r.ErrorMessage != null ? r.ErrorMessage : r.Exception?.Message)
                        .Where(r => !string.IsNullOrEmpty(r))
                        .ToList());
                    isErr = true;
                }
            }

            results.Rows.Add(dataRow);

            return isErr ? results : null;
        }

        public DataTable GetDataTable(Dictionary<string, string> data)
        {
            var result = new DataTable();
            var dataRow = result.NewRow();

            foreach (var item in data)
            {
                result.Columns.Add(new DataColumn(item.Key));
                dataRow[item.Key] = item.Value;
            }

            result.Rows.Add(dataRow);

            return result;
        }
    }
}
