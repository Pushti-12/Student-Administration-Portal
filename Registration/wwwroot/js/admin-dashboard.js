// =========================
// GET LOGIN INFORMATION
// =========================

const token =
    localStorage.getItem("jwtToken");


const loggedInUser =
    JSON.parse(
        localStorage.getItem("loggedInUser") || "null"
    );


// =========================
// CHECK LOGIN
// =========================

if (!token || !loggedInUser) {

    window.location.href = "/";

}


// =========================
// CHECK ADMIN ROLE
// =========================

if (
    loggedInUser &&
    loggedInUser.role &&
    loggedInUser.role.toUpperCase() !== "ADMIN"
) {

    const role =
        loggedInUser.role.toUpperCase();


    if (role === "TEACHER") {

        window.location.href =
            "/Teacher/Dashboard";

    }
    else if (role === "STUDENT") {

        window.location.href =
            "/Student/Dashboard";

    }
    else {

        window.location.href =
            "/";

    }
}


// =========================
// PROFILE ELEMENTS
// =========================

const adminNameElement =
    document.getElementById(
        "adminName"
    );


const adminAvatarElement =
    document.getElementById(
        "adminAvatar"
    );


const profileToggle =
    document.getElementById(
        "profileToggle"
    );


const profileDropdown =
    document.getElementById(
        "profileDropdown"
    );


// =========================
// GET ADMIN INITIAL
// =========================

function getAdminInitial(name) {

    if (
        !name ||
        String(name).trim() === ""
    ) {

        return "A";

    }


    return String(name)
        .trim()
        .charAt(0)
        .toUpperCase();

}


// =========================
// SET ADMIN NAME
// =========================

function setAdminName(name) {

    if (!adminNameElement)
        return;


    adminNameElement.textContent =
        name || "Admin";

}


// =========================
// SET ADMIN AVATAR
// =========================

function setAdminAvatar(
    profileImage,
    name
) {

    if (!adminAvatarElement)
        return;


    const initial =
        getAdminInitial(name);


    // Clear previous content.

    adminAvatarElement.innerHTML =
        "";


    // =========================
    // PROFILE IMAGE EXISTS
    // =========================

    if (
        profileImage &&
        String(profileImage).trim() !== ""
    ) {

        const image =
            document.createElement(
                "img"
            );


        image.src =
            profileImage;


        image.alt =
            "Admin Profile";


        image.style.width =
            "40px";


        image.style.height =
            "40px";


        image.style.objectFit =
            "cover";


        image.style.borderRadius =
            "50%";


        image.style.display =
            "block";


        image.onerror =
            function () {

                adminAvatarElement.innerHTML =
                    initial;

            };


        adminAvatarElement.appendChild(
            image
        );

    }

    // =========================
    // NO PROFILE IMAGE
    // =========================

    else {

        adminAvatarElement.textContent =
            initial;

    }

}


// =========================
// SHOW LOCAL DATA FIRST
// =========================

if (loggedInUser) {

    const localName =
        loggedInUser.name ||
        loggedInUser.Name ||
        "Admin";


    const localImage =
        loggedInUser.profileImage ||
        loggedInUser.ProfileImage ||
        "";


    setAdminName(
        localName
    );


    setAdminAvatar(
        localImage,
        localName
    );

}


// =========================
// LOAD LATEST ADMIN PROFILE
// FROM DATABASE
// =========================

async function loadAdminProfile() {

    if (!token)
        return;


    try {

        const response =
            await fetch(
                "/api/admin/me",
                {
                    method: "GET",

                    headers: {
                        "Authorization":
                            "Bearer " +
                            token,

                        "Content-Type":
                            "application/json"
                    }
                }
            );


        // =========================
        // AUTH ERROR
        // =========================

        if (
            response.status === 401 ||
            response.status === 403
        ) {

            localStorage.removeItem(
                "jwtToken"
            );


            localStorage.removeItem(
                "loggedInUser"
            );


            window.location.href =
                "/Auth/Login";


            return;

        }


        if (!response.ok) {

            console.error(
                "Unable to load admin profile. Status:",
                response.status
            );


            return;

        }


        const admin =
            await response.json();


        // =========================
        // GET LATEST VALUES
        // =========================

        const name =
            admin.name ||
            admin.Name ||
            "Admin";


        const email =
            admin.email ||
            admin.Email ||
            "";


        const profileImage =
            admin.profileImage ||
            admin.ProfileImage ||
            "";


        // =========================
        // UPDATE DASHBOARD
        // =========================

        setAdminName(
            name
        );


        setAdminAvatar(
            profileImage,
            name
        );


        // =========================
        // UPDATE LOCAL STORAGE
        // =========================

        if (loggedInUser) {

            loggedInUser.name =
                name;


            loggedInUser.email =
                email;


            loggedInUser.profileImage =
                profileImage;


            localStorage.setItem(
                "loggedInUser",
                JSON.stringify(
                    loggedInUser
                )
            );

        }

    }
    catch (error) {

        console.error(
            "Unable to load admin profile:",
            error
        );

    }

}


// =========================
// PROFILE DROPDOWN
// =========================

if (
    profileToggle &&
    profileDropdown
) {

    profileToggle.addEventListener(
        "click",
        function (event) {

            event.preventDefault();

            event.stopPropagation();


            profileDropdown.classList.toggle(
                "show"
            );

        }
    );


    document.addEventListener(
        "click",
        function (event) {

            if (
                !profileToggle.contains(
                    event.target
                ) &&
                !profileDropdown.contains(
                    event.target
                )
            ) {

                profileDropdown.classList.remove(
                    "show"
                );

            }

        }
    );

}


// =========================
// API REQUEST FUNCTION
// =========================

async function fetchData(url) {

    const response =
        await fetch(
            url,
            {
                method: "GET",

                headers: {
                    "Authorization":
                        `Bearer ${token}`,

                    "Content-Type":
                        "application/json"
                }
            }
        );


    if (!response.ok) {

        throw new Error(
            "Unable to load dashboard data."
        );

    }


    return await response.json();

}


// =========================
// LOAD DASHBOARD DATA
// =========================

async function loadDashboard() {

    try {

        // =========================
        // STUDENTS
        // =========================

        const students =
            await fetchData(
                "/api/students"
            );


        document.getElementById(
            "studentCount"
        ).textContent =
            students.length;


        // =========================
        // TEACHERS
        // =========================

        const teachers =
            await fetchData(
                "/api/teachers"
            );


        document.getElementById(
            "teacherCount"
        ).textContent =
            teachers.length;


        // =========================
        // COURSES
        // =========================

        const courses =
            await fetchData(
                "/api/courses"
            );


        document.getElementById(
            "courseCount"
        ).textContent =
            courses.length;

    }
    catch (error) {

        console.error(
            error
        );


        const errorMessage =
            document.getElementById(
                "errorMessage"
            );


        if (errorMessage) {

            errorMessage.textContent =
                "Unable to load dashboard data. Please try again.";


            errorMessage.style.display =
                "block";

        }

    }

}


// =========================
// LOGOUT
// =========================

async function logout() {

    try {

        await fetch(
            "/api/auth/logout",
            {
                method: "POST",

                headers: {
                    "Authorization":
                        `Bearer ${token}`,

                    "Content-Type":
                        "application/json"
                }
            }
        );

    }
    catch (error) {

        console.error(
            "Logout API error:",
            error
        );

    }
    finally {

        localStorage.removeItem(
            "jwtToken"
        );


        localStorage.removeItem(
            "loggedInUser"
        );


        window.location.href =
            "/";

    }

}


// =========================
// LOGOUT BUTTON
// =========================

const logoutButton =
    document.getElementById(
        "logoutBtn"
    );


if (logoutButton) {

    logoutButton.addEventListener(
        "click",
        logout
    );

}


// =========================
// INITIALIZE
// =========================

// Load latest admin data
// from database.

loadAdminProfile();


// Load existing dashboard
// statistics.

loadDashboard();