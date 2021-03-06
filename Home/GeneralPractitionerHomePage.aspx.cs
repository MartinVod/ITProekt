﻿using System;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;

public partial class Home_GeneralPractionerDefaultPage : System.Web.UI.Page
{

    private const string MSG_ERROR_READING_HOSPITAL = "[Something went wrong while reading hospital data!]";
    private const string MSG_ERROR_READING_PATIENTS = "[Something went wrong while reading patients data!]";
    private const string MSG_ERROR_READING_APPOINTMENTS = "[Something went wrong while reading appointments data!]";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsUserLoggedIn())
        {
            // Redirect to error page
            Response.Redirect("~/ErrorPages/NotLoggedIn.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (!IsGeneralPractionerLoggedIn())
        {
            Response.Redirect("~/ErrorPages/NotAuthorized.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (!Page.IsPostBack)
        {
            tabPersonalInfo.CssClass = "clicked";
            multiView.ActiveViewIndex = 0;
            ReadDoctorFromDb();
        }
    }

    protected void tabPersonalInfo_Click(object sender, EventArgs e)
    {
        tabPersonalInfo.CssClass = "clicked";
        tabGpPatients.CssClass = "initial";
        tabFreePatients.CssClass = "initial";
        tabAllAppointments.CssClass = "initial";
        multiView.ActiveViewIndex = 0;
    }

    protected void tabGpPatients_Click(object sender, EventArgs e)
    {
        tabPersonalInfo.CssClass = "initial";
        tabGpPatients.CssClass = "clicked";
        tabFreePatients.CssClass = "initial";
        tabAllAppointments.CssClass = "initial";
        multiView.ActiveViewIndex = 1;
        ReadGeneralPractionerPatients();
    }

    protected void tabFreePatients_Click(object sender, EventArgs e)
    {
        tabPersonalInfo.CssClass = "initial";
        tabGpPatients.CssClass = "initial";
        tabFreePatients.CssClass = "clicked";
        tabAllAppointments.CssClass = "initial";
        multiView.ActiveViewIndex = 2;
        ReadPatientsWithoutGp();
    }

    protected void tabAllAppointments_Click(object sender, EventArgs e)
    {
        tabPersonalInfo.CssClass = "initial";
        tabGpPatients.CssClass = "initial";
        tabFreePatients.CssClass = "initial";
        tabAllAppointments.CssClass = "clicked";
        multiView.ActiveViewIndex = 3;
        ReadAllAppointments();
    }

    private bool IsUserLoggedIn()
    {
        return Session["user_id"] != null && Session["user_type"] != null;
    }

    private bool IsGeneralPractionerLoggedIn()
    {
        UserType userType = (UserType)Session["user_type"];
        return userType == UserType.DOCTOR_GP;
    }

    private void ReadDoctorFromDb()
    {
        string id = (string)Session["user_id"];
        Doctor doctor = DBUtils.FindDoctorById(id);
        if (doctor != null)
        {
            lblName.Text = doctor.Name;
            lblSurname.Text = doctor.Surname;
            lblEmail.Text = doctor.Email;
            lblDoctorType.Text = "General practioner";
            Hospital hospital = DBUtils.FindHospitalById(doctor.HospitalId);
            if (hospital != null)
            {
                lblHospital.Text = hospital.Name;
                lblHospital.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                lblHospital.Text = MSG_ERROR_READING_HOSPITAL;
                lblHospital.ForeColor = System.Drawing.Color.DarkRed;
            }
        }
        else
        {
            // Redirect to error
            Response.Redirect("~/ErrorPages/Oops.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    private void ReadGeneralPractionerPatients()
    {
        string id = (string)Session["user_id"];
        DataSet dataSet = DBUtils.GetPatientsForGp(id);
        if (dataSet == null)
        {
            lblGpPatientsInfo.Text = MSG_ERROR_READING_PATIENTS;
        }
        else
        {
            gvGpPatients.DataSource = dataSet;
            gvGpPatients.DataBind();
            ViewState["gp_patients"] = dataSet;
            lblGpPatientsInfo.Text = "";
        }
    }

    private void ReadPatientsWithoutGp()
    {
        DataSet dataSet = DBUtils.GetPatientsWithoutGp();
        if (dataSet == null)
        {
            lblNoGpPatientsInfo.Text = MSG_ERROR_READING_PATIENTS;
        }
        else
        {
            gvPatientWithoutGp.DataSource = dataSet;
            gvPatientWithoutGp.DataBind();
            ViewState["no_gp_patients"] = dataSet;
            lblNoGpPatientsInfo.Text = "";
        }
    }

    private void ReadAllAppointments()
    {
        string id = (string)Session["user_id"];
        DataSet dataSet = DBUtils.GetAppointmentsForDoctor(id);
        if (dataSet == null)
        {
            lblAllAppointmentsInfo.Text = MSG_ERROR_READING_APPOINTMENTS;
        }
        else
        {
            gvAllAppointments.DataSource = dataSet;
            gvAllAppointments.DataBind();
            ViewState["all_appointments"] = dataSet;
            lblAllAppointmentsInfo.Text = "";
        }
    }

    protected void gvGpPatients_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvGpPatients.PageIndex = e.NewPageIndex;
        gvGpPatients.SelectedIndex = -1;
        DataSet dataSet = (DataSet)ViewState["gp_patients"];
        gvGpPatients.DataSource = dataSet;
        gvGpPatients.DataBind();
    }


    protected void gvGpPatients_SelectedIndexChanged(object sender, EventArgs e)
    {
        // TODO Redirect to patient details page
    }

    protected void gvPatientWithoutGp_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvPatientWithoutGp.PageIndex = e.NewPageIndex;
        gvPatientWithoutGp.SelectedIndex = -1;
        DataSet dataSet = (DataSet)ViewState["no_gp_patients"];
        gvPatientWithoutGp.DataSource = dataSet;
        gvPatientWithoutGp.DataBind();
    }

    protected void gvPatientWithoutGp_SelectedIndexChanged(object sender, EventArgs e)
    {
        // TODO Redirect to patient details page
    }

    protected void gvAllAppointments_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvAllAppointments.PageIndex = e.NewPageIndex;
        gvAllAppointments.SelectedIndex = -1;
        DataSet dataSet = (DataSet)ViewState["all_appointments"];
        gvAllAppointments.DataSource = dataSet;
        gvAllAppointments.DataBind();
    }

    protected void gvAllAppointments_SelectedIndexChanged(object sender, EventArgs e)
    {
        string id = gvAllAppointments.SelectedDataKey.Value.ToString();
        Response.Redirect("~/Appointments/AppointmentDetails.aspx?appId=" + Server.UrlEncode(id));
    }

    protected void btnSpecialistAppointment_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Appointments/AddAppointment.aspx");
    }
}