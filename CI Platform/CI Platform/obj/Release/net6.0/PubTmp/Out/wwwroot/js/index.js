////const { Modal } = require("../lib/bootstrap/dist/js/bootstrap.bundle")

////let items = document.getElementsByClassName("item")
////let cards = document.getElementsByClassName("thumbnail")
////let list = document.getElementById("list")
////let grid = document.getElementById("grid")
////let allchoices = []
////function listview() {
////    for (var i = 0; i < items.length; i++) {
////        items[i].classList.remove("col-lg-4")
////        items[i].classList.add("col-lg-12")
////        items[i].classList.remove("col-md-6")
////        items[i].classList.add("col-md-12")
////        items[i].classList.remove("col-sm-6")
////        items[i].classList.add("col-sm-12")
////        items[i].classList.add("mb-5")
////        cards[i].classList.remove("card")
////        cards[i].classList.add("d-flex")
////    }
////    grid.classList.remove("view")
////    list.classList.add("view")
////    list.style.marginLeft = 20 + "px";
////}
////function gridview() {
////    for (var i = 0; i < items.length; i++) {
////        items[i].classList.add("col-lg-4")
////        items[i].classList.remove("col-lg-12")
////        items[i].classList.add("col-md-6")
////        items[i].classList.remove("col-md-12")
////        items[i].classList.add("col-sm-6")
////        items[i].classList.remove("col-sm-12")
////        items[i].classList.remove("mb-5")
////        cards[i].classList.remove("d-flex")
////        cards[i].classList.add("card")
////    }
////    grid.classList.add("view")
////    list.classList.remove("view")
////    list.style.marginLeft = 0 + "px";
////}

////function setfilters(a) {
////    if (document.getElementById(a).checked) {
////        allchoices.push(a)
////        console.log(allchoices)
////    }
////    else {
////        allchoices.pop(1, allchoices.indexOf(a))
////        console.log(allchoices)
////    }
////}
////function getfilters() {
////    return allchoices
////}


//function getCity() {
//    var countryId = $('#countryId').find(":selected").val();
//    debugger;
//    $.ajax({
//        url: "/Platform/GetCity",
//        method: "GET",
//        data: {
//            "countryId": countryId
//        },
//        success: function (data) {
//            data = JSON.parse(data);
//            $("#selectCityList").empty();
//            data.forEach((name) => {
//                document.getElementById("selectCityList").innerHTML += `
//        <option value=${name} >${name.Name}</option>
//        `;
//            })
//        },
//        error: function (request, error) {
//            console.log(error);
//        }
//    })
//}

//function Validate() {
//    var password = document.getElementById("txtPassword").value;
//    var confirmPassword = document.getElementById("txtConfirmPassword").value;
//    if (password != confirmPassword) {
//        alert("Passwords do not match.");
//        return false;
//    }
//    return true;
//}

$('.listItem').hide();
$('#grid').click(function () {
    $('.gridItem').show();
    $('.listItem').hide();
    localStorage.setItem("lastVisible", "list");
})

$('#list').click(function () {
    $('.listItem').show();
    $('.gridItem').hide();
    localStorage.setItem("lastVisible", "grid");
})

$(document).ready(function (a) {
    //---Convet-Grid-List---
    localStorage.setItem("lastVisible", "grid");
    var windowWidth = $(window).width();
    // Check if the window width is less than 1440px
    if (windowWidth < 991) {
        // Hide the list view and show the gird view
        $('.listItem').hide();
        $('.gridItem').show();
    }
    $(window).resize(function (e) {
        // Get the new window width
        var newWindowWidth = $(window).width();

        // Check if the window width is less than 1440px
        if (newWindowWidth < 991) {
            // Hide the list view and show the gird view
            $('.listItem').hide();
            $('.gridItem').show();
            $("#list").hide();
            $("#grid").hide();
        }
        else {
            $("#list").show();
            $("#grid").show();
            // Show the list view and hide the gird view
            if (localStorage.getItem("lastVisible") == "list") {
                $('.gridItem').hide();
                $('.listItem').show();
            }
        }
    });


});

function getCity() {

    var countryId = [];

    $('input[name="country"]:checked').each(function () {
        countryId.push(this.id);
    });

    console.log(countryId);
    $.ajax({

        url: "/Platform/getCity",
        method: "POST",

        data: {
            "countryId": countryId
        },

        success: function (data) {

            data = JSON.parse(data);
            $("#selectCityList").empty();
            $("#selectCityList1").empty();

            data.forEach((name) => {

                document.getElementById("selectCityList").innerHTML += `
                  <li>
                       <input type="checkbox" class="form-check-input mx-auto ms-1 city_${name.CityId}" name="city"  value="${name.Name}" id="${name.CityId}"/>
                       <label class="form-check-label ps-2 ms-3" for="${name.CityId}">
                          ${name.Name}
                       </label>
                  </li>

                     
                `;

                document.getElementById("selectCityList1").innerHTML += `
                  <li>
                       <input type="checkbox" class="form-check-input mx-auto ms-1 city_${name.CityId}" name="city"  value="${name.Name}" id="${name.CityId}"/>
                       <label class="form-check-label ps-2 ms-3" for="${name.CityId}">
                          ${name.Name}
                       </label>
                  </li>

                     
                `;

            })


        },
        error: function (request, error) {
            console.log(error);
        }

    })
}

function searchMission(pg) {

    var search = $('input[name="search"]').val();
    var countries = [];
    var cities = [];
    var themes = [];
    var skills = [];
    var pgSize = $('#pager').val();
    if (pgSize == 0) {
        pgSize = $('#pagerList').val();
    }

    if (pg == undefined) {
        pg = 1;
    }
    console.log(pg);
    var sort = document.getElementById("sort").value;


    $('input[name="country"]:checked').each(function () {
        countries.push(this.id);
    });
    $('input[name="city"]:checked').each(function () {
        cities.push(this.id);
    });
    $('input[name="theme"]:checked').each(function () {
        themes.push(this.id);
    });
    $('input[name="skill"]:checked').each(function () {
        skills.push(this.id);
    });

    $.ajax({

        url: "/Platform/Search",
        method: "POST",

        data: {
            "search": search,
            "countries": countries,
            "cities": cities,
            "themes": themes,
            "skills": skills,
            "sort": sort,
            "pg": pg,
            "pgSize": pgSize

        },
        success: function (data) {

            $('#missions').html(data);
            $('#pager').val(pgSize);
        },
        error: function (request, error) {

            console.log(error);
        }

    })

    $.ajax({

        url: "/Platform/SearchList",
        method: "POST",

        data: {
            "search": search,
            "countries": countries,
            "cities": cities,
            "themes": themes,
            "skills": skills,
            "sort": sort,
            "pg": pg,
            "pgSize": pgSize

        },
        success: function (data) {

            $('#missionsList').html(data);
            $('#pagerList').val(pgSize);
        },
        error: function (request, error) {

            console.log(error);
        }

    })
}

function filterBadges() {

    $("#filter-button").empty();
    $('input[name="country"]:checked').each(function () {
        document.getElementById("filter-button").innerHTML += `
<button class="filter rounded-pill border" id="${this.value}" >
<div style="width:max-content">${this.value} <i onclick="removeFilter(${this.id},'country')" class="bi bi-x"></i></div>
</button>
`
    });
    $('input[name="city"]:checked').each(function () {

        document.getElementById("filter-button").innerHTML += `
<button class="filter rounded-pill border" id="${this.value}" >
<div style="width:max-content">${this.value} <i onclick="removeFilter(${this.id},'city')" class="bi bi-x"></i></div>
</button>
`
    });
    $('input[name="theme"]:checked').each(function () {

        document.getElementById("filter-button").innerHTML += `
<button class="filter rounded-pill border" id="${this.value}">
<div style="width:max-content">${this.value} <i onclick="removeFilter(${this.id},'theme')" class="bi bi-x"></i></div>
</button>
`
    });

    $('input[name="skill"]:checked').each(function () {

        document.getElementById("filter-button").innerHTML += `
<button class="filter rounded-pill border" id="${this.value}">
<div style="width:max-content">${this.value} <i onclick="removeFilter(${this.id},'skill')" class="bi bi-x"></i></div>
</button>
`
    });

    document.getElementById("filter-button").innerHTML += `
<button class="clearall p-0 rounded-pill border" onclick="clearAll()">Clear all</button>
`
}

function removeFilter(checkboxId, type) {
    console.log("rm" + checkboxId);
    if (type == 'country') {
        $(".country_" + checkboxId).prop("checked", false);
    }
    if (type == 'city') {
        $(".city_" + checkboxId).prop("checked", false);
    }
    if (type == 'theme') {
        $(".theme_" + checkboxId).prop("checked", false);
    }
    if (type == 'skill') {
        $(".skill_" + checkboxId).prop("checked", false);
    }

    searchMission();
    filterBadges();

}

function clearAll() {
    $('input[name="country"]:checked').each(function () {

        $(".country_" + this.id).prop("checked", false);
    });
    $('input[name="city"]:checked').each(function () {

        $(".city_" + this.id).prop("checked", false);
    });
    $('input[name="theme"]:checked').each(function () {

        $(".theme_" + this.id).prop("checked", false);
    });
    $('input[name="skill"]:checked').each(function () {

        $(".skill_" + this.id).prop("checked", false);
    });

    filterBadges();
    searchMission();
    $(".clearall").addClass("d-none");
}

function changePgIndex(type) {
    debugger
    var pgSize;
    if (type == "grid") {
        pgSize = $('#pager').val();
    }
    if (type == "list") {
        pgSize = $('#pagerList').val();
    }
    searchMission();
    $.ajax({

        url: '/Platform/gridView',
        method: "POST",
        data: {
            'pgSize': pgSize,
            'type': type,
        },
        success: function (data) {
            if (type == 'grid') {
                $('#missions').html(data);
                $('#pager').val(pgSize);

            }
            if (type == 'list') {
                $('#missionsList').html(data);
                $('#pagerList').val(pgSize);

            }
        },

    });

}

function addToFavourite(missionId) {
    debugger
    $.ajax({

        url: '/Platform/addToFavourite',
        method: "POST",
        data: {
            'missionId': missionId,
        },
        success: function (data) {
            debugger
            toastr.options = {
                "positionClass": "toast-bottom-right"
            }
            if (data == true) {
                debugger
                $("#favBtn11").text("Remove from Favourite");
                $("#addFav_" + missionId).addClass("text-danger");
                $("#missionFav").addClass("text-danger");
                toastr.success("Added Favourite Successfully...");
                location.reload();
            }
            else {
                debugger
                $("#favBtn11").text("Add to Favourite");
                $("#addFav_" + missionId).removeClass("text-danger");
                //$("#addFav_" + missionId).css("color","white");
                $("#missionFav").removeClass("text-danger");
                toastr.error("Remove Favourite Successfully...");
                location.reload();

            }
        },
        error: function (request, error) {
            console.log("function not working");
            alert('Error');
        },

    });
}

function applyMission(missionId) {
    debugger
    $.ajax({

        url: '/Platform/applyMission',
        method: "POST",
        data: {
            'missionId': missionId,
        },
        success: function (missions) {
            debugger

            toastr.options = {
                "positionClass": "toast-bottom-right"
            }

            if (missions == 1) {
                toastr.success("Applied Successfully...")
            }
            else if (missions == 2) {
                toastr.info("wait for approval...")
            }
            else {
                toastr.error("You've already Applied... ")
            }

        },
        error: function (request, error) {
            console.log("function not working");
            alert('Error');
        },

    });

}

function addComments(missionId) {

    var comment = $('#comment').val();
    if (comment == null || comment == "") {
        $('#comment-msg').html("Please Enter Comments..");
        $('#comment-msg').css("color", "red");
    }
    else {

        $.ajax({

            url: '/Platform/addComments',
            method: "POST",
            data: {
                'comment': comment,
                'missionId': missionId
            },
            success: function (data) {
                $('#comment-msg').html("Comments added & Wait for approval of Comments..")
                $('#comment-msg').css("color", "green");
                $('#comment-msg').val('');

            }
        })
    }
}

function sendMail(missionId) {

    var emailList = [];

    $('input[name="email"]:checked').each(function () {
        emailList.push(this.id);
    });

    $.ajax({

        url: "/Platform/RecommendCoWorker",
        method: "POST",

        data: {
            "emailList": emailList,
            "missionId": missionId
        },
        success: function (data) {
            if (data) {

                toastr.options = {
                    "positionClass": "toast-bottom-right"
                }
                toastr.success("Mail Successfully...")

                //// Loop through each ID in the emailList array
                //for (var i = 0; i < emailList.length; i++) {
                //    // Get the checkbox element with the current ID
                //    var checkbox = $('#' + emailList[i]);

                //    // Set the checked property of the checkbox to false
                //    checkbox.prop('checked', false);
                //}


            }
        },
        error: function (request, error) {

            console.log(error);
        }

    })
}

function sendStoryMail(storyId) {

    var emailList = [];

    $('input[name="email"]:checked').each(function () {
        emailList.push(this.id);
    });

    $.ajax({

        url: "/Story/RecommendCoWorker",
        method: "POST",

        data: {
            "emailList": emailList,
            "storyId": storyId
        },
        success: function (data) {
            if (data) {

                toastr.options = {
                    "positionClass": "toast-bottom-right"
                }
                toastr.success("Mail Successfully...")
            }
        },
        error: function (request, error) {

            console.log(error);
        }

    })
}

function rVolunteer(pg, missionId) {

    if (pg == undefined) {
        pg = 1;
    }
    console.log(pg);

    $.ajax({

        url: "/Platform/rVolunteer",
        method: "POST",
        data: {
            "pg": pg,
            "missionId": missionId,
        },
        success: function (data) {
            $('#recentVolunteers').html(data);
        },
        error: function (request, error) {
            console.log(error);
        }
    })
}

function searchStory(pg) {
    debugger
    var search = $('input[name="search"]').val();
    var countries = [];
    var cities = [];
    var themes = [];
    var skills = [];
    if (pg == undefined) {
        pg = 1;
    }
    console.log(pg);

    $('input[name="country"]:checked').each(function () {
        countries.push(this.id);
    });
    $('input[name="city"]:checked').each(function () {
        cities.push(this.id);
    });
    $('input[name="theme"]:checked').each(function () {
        themes.push(this.id);
    });
    $('input[name="skill"]:checked').each(function () {
        skills.push(this.id);
    });

    $.ajax({

        url: "/Story/Search",
        method: "POST",

        data: {
            "search": search,
            "countries": countries,
            "cities": cities,
            "themes": themes,
            "skills": skills,
            "pg": pg
        },
        success: function (data) {
            debugger
            $('#stories').html(data);

        },
        error: function (request, error) {

            console.log(error);
        }

    })
}

var isSamePass;
function checkOldpassword() {
    debugger
    var oldPassword = $('#OldPassword').val();
    console.log(oldPassword)
    var isValid = true;
    if (oldPassword == '') {
        $('#oldPasswordError').text('Please enter your old password.');
        isValid = false;
    } else {
        $('#oldPasswordError').text('');
    }
    if (isValid) {
        $.ajax({
            url: '/User/userEditProfile',
            type: 'POST',
            dataType: 'json',
            data: {
                OldPassword: oldPassword
            },
            success: function (data) {
                if (data == true) {
                    isSamePass = true;
                    //alert('Password changed successfully.');
                } else {
                    $('#oldPasswordError').text('Please enter Correct Old password.');
                    isSamePass = false;
                }
            },
            error: function (error) {
                alert('An error occurred while changing your password: ' + error);
            }
        });
    }
}

function changePassword() {
    debugger
    var oldPassword = $('#OldPassword').val();
    var password = $('#Password').val();
    var cnfPassword = $('#CnfPassword').val();
    console.log(password)
    var isValid = true;
    if (oldPassword == '') {
        $('#oldPasswordError').text('Please enter your old password.');
        isValid = false;
    } else {
        $('#oldPasswordError').text('');
        isValid = isSamePass;
    }
    if (password == '') {
        $('#passwordError').text('Please enter a new password.');
        isValid = false;
    } else {
        if (/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])(?=.*[a-zA-Z]).{8,}$/.test(password)) {
            $('#passwordError').text('');
        } else {
            $('#passwordError').text('Enter a strong password with at least 8 characters, including at least 1 uppercase letter,1 lowercase letter,1 number,& 1 special character');
            isValid = false;
        }
    }
    if (cnfPassword == '') {
        $('#cnfPasswordError').text('Please confirm your new password.');
        isValid = false;
    } else if (password != cnfPassword) {
        $('#cnfPasswordError').text('New password and confirm password do not match.');
        isValid = false;
    } else {
        $('#cnfPasswordError').text('');
    }

    if (isValid) {
        $.ajax({
            url: '/User/userEditProfile',
            type: 'POST',
            dataType: 'json',
            data: {
                Password: password
            },
            success: function (data) {
                toastr.options = {
                    "positionClass": "toast-bottom-right"
                }
                if (data == true) {
                    toastr.success("Password Changed Successfully...");
                    $('#passwordModal').modal('hide');
                    $('#changePasswordForm')[0].reset();

                } else {
                    // Password change failed
                    alert("errorr...");
                }
            },
            error: function (xhr, status, error) {
                // Handle error
                alert('An error occurred while changing your password: ' + error);
            }
        });

    }
}

function getUserCity(x) {
    debugger
    var countryId;
    if (x != 0) {
        countryId = x;
    }
    else {
        countryId = document.getElementById("country").value;
    }

    $.ajax({
        url: "/Platform/getCity",
        method: "Post",
        data: { "countryId": countryId },
        success: function (data) {
            data = JSON.parse(data);
            $("#selectCityList").empty();
            data.forEach((name) => {
                document.getElementById("selectCityList").innerHTML += `

<option class="form-check" name="CityId" value="${name.CityId}">${name.Name}</option>

`;
            })
        },
        error: function (request, error) {
            console.log(error);
        }
    })
};

function addSelectedSkill() {
    console.log("Add TO Skill Text");
    var selectValue = document.getElementById("selected_features");
    console.log(selectValue);

    var list = selectValue.getElementsByTagName("option");
    console.log(list);
    $("#addSkillsHere").empty();

    for (i = 0; i < list.length; i++) {
        document.getElementById("addSkillsHere").innerHTML +=
            `<li>
<input asp-for="skillsToAdd" name="skillsToAdd" id="addSkillList" type="hidden"value="${list[i].value}" \ > ${list[i].text}
</li>`;
        console.log(list[i].text);
    }
    $('#exampleModal1').modal('hide');


}

function contactUs() {
    debugger
    var FirstName = $('#FirstName').val();
    var Email = $('#Email').val();
    var subject = $('#subject').val();
    var Message = $('#Message').val();
    var isValid = true;

    if (isValid) {
        $.ajax({
            url: '/User/userEditProfile',
            type: 'POST',
            dataType: 'json',
            data: {
                FirstName: FirstName,
                Email: Email,
                subject: subject,
                Message: Message
            },
            success: function (data) {
                toastr.options = {
                    "positionClass": "toast-bottom-right"
                }
                if (data == true) {
                    toastr.success("Your data has been sent...");
                    $('#exampleModal2').modal('hide');
                    $('#contactUsForm')[0].reset();

                } else {
                    alert("errorr...");
                }
            },
            error: function (error) {
                alert('An error occurred while sending data: ' + error);
            }
        });

    }
}

function uploadProfile() {
    debugger
    var image = document.getElementById('profile-image').files[0]
    var fr = new FileReader()
    fr.onload = () => {
        $('#old-profile-image').attr('src', fr.result)
    }
    fr.readAsDataURL(image)
}

function clearModal() {
    $('#timeSheetId').val('');
    $('#timeTitle').val(0);
    $('#timeDate').val('');
    $('#timeHour').val('');
    $('#timeMinute').val('');
    $('#timeMessage').val('');
    $('#goalTitle').val(0);
    $('#goalAction').val('');
    $('#goalDate').val('');
    $('#goalMessage').val('');
    $("#timeTitle").prop("disabled", false);
    $("#goalTitle").prop("disabled", false);

    var spanElements = document.querySelectorAll("#timeModal span");
    for (var i = 0; i < spanElements.length; i++) {
        spanElements[i].textContent = "";
    }
    var spanElements = document.querySelectorAll("#goalModal span");
    for (var i = 0; i < spanElements.length; i++) {
        spanElements[i].textContent = "";
    }

}

function addGoalSheets() {
    debugger
    var MissionId = $('#goalTitle').val();
    var goalAction = $('#goalAction').val();
    var date = $('#goalDate').val();
    var Message = $('#goalMessage').val();
    var TimesheetId = $('#timeSheetId').val();
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    const currentDate = `${year}-${month}-${day}`;

    var isValid = true;
    if (MissionId == 0) {
        $('#goalTitleError').text('Please select Mission Title');
        isValid = false;
    } else {
        $('#goalTitleError').text('');
    }
    if (goalAction == '') {
        $('#goalActionError').text('Enter Value');
        isValid = false
    }
    else {
        if (goalAction <= 0) {
            $('#goalActionError').text('Enter positive value');
            isValid = false;
        } else {
            $('#goalActionError').text('');
        }
    }
    if (date == '') {
        $('#goalDateError').text('Please select Date');
        isValid = false;
    } else {
        if (date > currentDate) {
            $('#goalDateError').text('Please select before Today\'s Date');
            isValid = false;
        } else {
            $('#goalDateError').text('');
        }
    }
    if (Message == '') {
        $('#goalMessageError').text('Please enter message.');
        isValid = false;
    } else {
        $('#goalMessageError').text('');
    }
    if (isValid) {
        $.ajax({
            url: '/User/addTimeSheets',
            type: 'POST',
            dataType: 'json',
            data: {
                MissionId: MissionId,
                goalAction: goalAction,
                Date: date,
                Message: Message,
                TimesheetId: TimesheetId
            },
            success: function (data) {
                toastr.options = {
                    "positionClass": "toast-bottom-right"
                }
                if (data == true) {
                    toastr.success("Goal Sheet added Successfully...");
                    $('#goalModal').modal('hide');
                    $('#goalTitle').val();
                    $('#goalAction').val();
                    $('#goalDate').val();
                    $('#goalMessage').val();

                    setTimeout(function () {
                        location.reload();
                    }, 500);
                } else {
                    $('#goalDateError').text('You can not select same date');
                }
            },
            error: function (error) {
                alert('An error occurred while sending data: ' + error);
            }
        });

    }
}

function checkSameDate() {
    debugger
    var oldDate = $('#timeDate').val();
    var oldgoalDate = $('#goalDate').val();
    var TimesheetId = $('#timeSheetId').val();

    var MissionId = $('#timeTitle').val();
    if (MissionId == null) {
        MissionId = $('#goalTitle').val();
        oldDate = oldgoalDate;
    }
    if (MissionId == 0) {
        MissionId = $('#goalTitle').val();
        oldDate = oldgoalDate;
    }
    console.log(oldDate);
    var isValid = true;

    if (MissionId == 0) {
        $('#timeTitleError').text('Please select title first...');
        $('#goalTitleError').text('Please select title first...');
        $('#timeDate').val('');
        $('#goalDate').val('');
        isValid = false;
    }
    else {
        $('#timeTitleError').text('');
        $('#goalTitleError').text('');
    }
    if (oldDate != '') {
        $('#timeDateError').text('');
    }
    else if (oldgoalDate != '') {
        $('#goalDateError').text('');
    } else {
        $('#timeDateError').text('Please select date..');
        $('#goalDateError').text('Please select date..');
        isValid = false;
    }
    if (isValid) {
        $.ajax({
            url: '/User/checkSameDate',
            type: 'POST',
            dataType: 'json',
            data: {
                Date: oldDate,
                MissionId: MissionId,
                TimesheetId: TimesheetId
            },
            success: function (data) {

                //alert('Password changed successfully.');
                if (data == false) {
                    $('#timeDate').val('');
                    $('#goalDate').val('');
                    $('#timeDateError').text('You can\'t select same date or select date after start\'s date & before today\'s date');
                    $('#goalDateError').text('You can\'t select same date or select date after start\'s date & before today\'s date');
                } else {
                    $('#timeDateError').text('');
                    $('#goalDateError').text('');

                }

            },
            error: function (error) {
                alert('An error occurred while selecting date: ' + error);
            }
        });
    }
}

function addTimeSheets() {
    debugger
    var MissionId = $('#timeTitle').val();
    var date = $('#timeDate').val();
    var timeHour = $('#timeHour').val();
    var timeMinute = $('#timeMinute').val();
    var Message = $('#timeMessage').val();
    var TimesheetId = $('#timeSheetId').val();
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    const currentDate = `${year}-${month}-${day}`;

    console.log(TimesheetId);
    var isValid = true;

    if (MissionId == 0) {
        $('#timeTitleError').text('Please select Mission Title');
        isValid = false;
    } else {
        $('#timeTitleError').text('');
    }
    if (date == '') {
        $('#timeDateError').text('Please select Date');
        isValid = false;
    } else {
        if (date > currentDate) {
            $('#timeDateError').text('Please select before Today\'s Date');
            isValid = false;
        } else {
            $('#timeDateError').text('');
        }
    }
    if (timeHour == '') {
        $('#timeHourError').text('Enter Hours');
        isValid = false;
    } else {
        if (timeHour <= 0 || timeHour >= 23) {
            $('#timeHourError').text('Enter value between 0 to 23');
            isValid = false;
        } else {
            $('#timeHourError').text('');
        }
    }
    if (timeMinute == '') {
        $('#timeMinuteError').text('Enter Minutes');
        isValid = false;
    } else {
        if (timeMinute <= 0 || timeMinute >= 59) {
            $('#timeMinuteError').text('Enter value between 0 to 59');
            isValid = false;
        } else {
            $('#timeMinuteError').text('');
        }
    }
    if (Message == '') {
        $('#timeMessageError').text('Please enter message.');
        isValid = false;
    } else {
        $('#timeMessageError').text('');
    }
    if (isValid) {
        $.ajax({
            url: '/User/addTimeSheets',
            type: 'POST',
            dataType: 'json',
            data: {
                MissionId: MissionId,
                Date: date,
                timeHour: timeHour,
                timeMinute: timeMinute,
                Message: Message,
                TimesheetId: TimesheetId
            },
            success: function (data) {
                toastr.options = {
                    "positionClass": "toast-bottom-right"
                }
                if (data == true) {
                    toastr.success("Time Sheet added Successfully...");
                    $('#timeModal').modal('hide');
                    $('#timeTitle').val('');
                    $('#timeDate').val('');
                    $('#timeHour').val('');
                    $('#timeMinute').val('');
                    $('#timeMessage').val('');
                    setTimeout(function () {
                        location.reload();
                    }, 500);
                } else {
                    $('#timeDateError').text('You can not select same date');
                }
            },
            error: function (error) {
                alert('An error occurred while sending data: ' + error);
            }
        });

    }
}

function getSheets(TimesheetId) {
    var spanElements = document.querySelectorAll("#timeModal span");
    for (var i = 0; i < spanElements.length; i++) {
        spanElements[i].textContent = "";
    }
    var spanElements = document.querySelectorAll("#goalModal span");
    for (var i = 0; i < spanElements.length; i++) {
        spanElements[i].textContent = "";
    }
    $.ajax({
        url: "/User/getTimeSheets",
        method: "Post",
        data: { "TimesheetId": TimesheetId },
        success: function (data) {
            //data = JSON.parse(data);
            console.log(data);
            if (data.time != null) {

                var timeParts = data.time.split(':'); // Assuming data.time is in the format 'hh:mm:ss'
                var hours = parseInt(timeParts[0]);
                var minutes = parseInt(timeParts[1]);
            }

            var datee = new Date(data.dateVolunteered);
            datee.setMinutes(datee.getMinutes() + 1440);
            var dateString = datee.toISOString().split('T')[0];

            $('#timeTitle').val(data.missionId);
            $('#timeDate').val(dateString);
            $('#timeHour').val(hours);
            $('#timeMinute').val(minutes);
            $('#timeMessage').val(data.notes);
            $('#timeSheetId').val(data.timesheetId);
            $('#goalTitle').val(data.missionId);
            $('#goalAction').val(data.action);
            $('#goalDate').val(dateString);
            $('#goalMessage').val(data.notes);
            $("#timeTitle").prop("disabled", true);
            $("#goalTitle").prop("disabled", true);

        },
        error: function (request, error) {
            console.log(error);
        }
    })

}

function deleteSheets() {

    debugger
    var TimesheetId = $('#timeSheetId').val();
    console.log(TimesheetId);
    $.ajax({
        url: '/User/addTimeSheets',
        type: 'POST',
        dataType: 'json',
        data: {
            TimesheetId: TimesheetId
        },
        success: function (data) {
            toastr.options = {
                "positionClass": "toast-bottom-right"
            }
            if (data == true) {
                toastr.success("Time Sheet Deleted Successfully...");
                $('#deleteTimeModal').modal('hide');
                setTimeout(function () {
                    location.reload();
                }, 500);
            } else {
                alert("errorr...");
            }
        },
        error: function (error) {
            alert('An error occurred while sending data: ' + error);
        }
    });

}

function searchOnAdmin(pg, type) {
    debugger
    console.log("abc");
    var search = $('input[name="search"]').val();
    if (search == "") {
        search = $('input[name="searchCms"]').val();
    }
    if (search == "") {
        search = $('input[name="searchMission"]').val();
    }
    if (search == "") {
        search = $('input[name="searchTheme"]').val();
    }
    if (search == "") {
        search = $('input[name="searchSkill"]').val();
    }
    if (search == "") {
        search = $('input[name="searchApplication"]').val();
    }
    if (search == "") {
        search = $('input[name="searchStory"]').val();
    }
    if (search == "") {
        search = $('input[name="searchBanner"]').val();
    }

    if (pg == undefined) {
        pg = 1;
    }

    if (type == undefined) {
        type = 'user';
    }

    debugger;
    $.ajax({
        url: "/Admin/Search",
        method: "POST",
        data: {
            "search": search,
            "pg": pg,
            "type": type
        },
        success: function (data) {
            debugger
            if (type == 'user') {
                $('#users').html(data);
            }
            else if (type == 'cms') {
                $('#cms').html(data);
            }
            else if (type == 'mission') {
                $('#missions').html(data);
            }
            else if (type == 'theme') {
                $('#themes').html(data);
            }
            else if (type == 'skill') {
                $('#skills').html(data);
            }
            else if (type == 'application') {
                $('#applications').html(data);
            }
            else if (type == 'story') {
                $('#stories').html(data);
            }
            else if (type == 'banner') {
                $('#banneres').html(data);
            }
        },
        error: function (request, error) {
            console.log(error);
        }
    })

}

var sameData = true;
function checkSameMail() {
    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test($('#email').val())) {
        var email = $('#email').val();
        $.ajax({
            url: '/Admin/checkSameData',
            type: 'POST',
            dataType: 'json',
            data: {
                Email: email,
            },
            success: function (data) {
                if (data == true) {
                    $('#emailError').text('Already Exists!  Enter another mail ID.');
                    sameData = false;

                } else {
                    $('#emailError').text('');
                    sameData = true;
                }
            }
        });
    } else {
        $('#emailError').text('Enter a valid email address');
        sameData = false;
    }
}

function checkSameOrder() {
    var sortOrder = $('#sortOrder').val()
    var bannerId = $('#bannerId').val();

    $.ajax({
        url: '/Admin/checkSameData',
        type: 'POST',
        dataType: 'json',
        data: {
            sortOrder: sortOrder,
            bannerId: bannerId
        },
        success: function (data) {
            if (data == true) {
                $('#sortOrderError').text('Already Exists!  Enter another Number.');
                sameData = false;
            } else {
                $('#sortOrderError').text('');
                sameData = true;
            }
        }
    });
}

function addViaAdmin(type) {
    debugger
    var s1 = true;
    var userData = new FormData();
    userData.append('MissionType', type);
    if (type == 'user') {
        userData.append('FirstName', $('#firstName').val());
        userData.append('LastName', $('#lastName').val());
        userData.append('UserName', $('#email').val());
        userData.append('Password', $('#password').val());
        userData.append('UserId', $('#phoneNumber').val());
        userData.append('Title', $('#role').val());
        userData.append('StoryTitle', $('#getrole').val());
        userData.append('CrudId', $('#userId').val());
        userData.append('Slug', $('#getemployeeId').val());
        userData.append('Theme', $('#getdepartment').val());
        userData.append('Status', $('#statusUser').val());
        var a = document.getElementById("profile-image");;
        var b = a.files;
        for (var i = 0; i < b.length; i++) {
            userData.append('bannerPhoto', b[i]);
        }

        if ($('#userId').val() != '') {
            if ($('#getemployeeId').val() == '') {
                $('#getemployeeIdError').text('Enter EmployeeId.');
                s1 = false;
            } else {
                $('#getemployeeIdError').text('');
            }
            if ($('#getdepartment').val() == '') {
                $('#getdepartmentError').text('Enter Department.');
                s1 = false;
            } else {
                $('#getdepartmentError').text('');
            }
        }
        else {

            if ($('#firstName').val() == '') {
                $('#firstNameError').text('Enter FirstName.');
                s1 = false;
            } else {
                $('#firstNameError').text('');
            }
            if ($('#lastName').val() == '') {
                $('#lastNameError').text('Enter LastName.');
                s1 = false;
            } else {
                $('#lastNameError').text('');
            }



            if ($('#email').val() == '') {
                $('#emailError').text('Enter Email');
                s1 = false;
            } else {
                s1 = sameData;
            }
            if ($('#password').val() == '') {
                $('#passwordError').text('Enter Password');
                s1 = false;
            } else {
                if (/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])(?=.*[a-zA-Z]).{8,}$/.test($('#password').val())) {
                    $('#passwordError').text('');
                } else {
                    $('#passwordError').text('Enter a strong password with at least 8 characters, including at least 1 uppercase letter,1 lowercase letter,1 number,& 1 special character');
                    s1 = false;
                }
            }
            if ($('#phoneNumber').val() == '') {
                $('#phoneNumberError').text('Enter PhoneNumber');
                s1 = false;
            } else {
                if (/^[1-9]\d{9}$/.test($('#phoneNumber').val())) {
                    $('#phoneNumberError').text('');
                } else {
                    $('#phoneNumberError').text('Enter a valid 10-digit mobile number starting with a digit between 1 and 9');
                    s1 = false;
                }
            }
        }

    }
    if (type == 'cms') {
        userData.append('Title', $('#title').val());
        userData.append('Description', $('#description').val());
        userData.append('Slug', $('#slug').val());
        userData.append('Status', $('#statusCms').val());
        userData.append('CrudId', $('#CmsPageId').val());
        if ($('#title').val() == '') {
            $('#titleError').text('Enter Title.');
            s1 = false;
        } else {
            $('#titleError').text('');
        }
        if ($('#description').val() == '') {
            $('#descriptionError').text('Enter Description.');
            s1 = false;
        } else {
            $('#descriptionError').text('');
        }
        if ($('#slug').val() == '') {
            $('#slugError').text('Enter Slug.');
            s1 = false;
        } else {
            $('#slugError').text('');
        }
        if ($('#statusCms').val() == 0) {
            $('#statusCmsError').text('Select status.');
            s1 = false;
        } else {
            $('#statusCmsError').text('');
        }
    }
    if (type == 'mission') {
        userData.append('Title', $('#missionTitle').val());
        userData.append('Description', $('#missionDescription').val());
        userData.append('ShortDescription', $('#missionShortDescription').val());
        userData.append('CountryId', $('#country').val());
        userData.append('CityId', $('#selectCityList').val());
        userData.append('Mission', $('#missionType').val());
        userData.append('OrganizationName', $('#missionOrganisationName').val());
        userData.append('OrganizationDescription', $('#missionOrganisationDetail').val());
        userData.append('StartDate', $('#missionStartDate').val());
        userData.append('EndDate', $('#missionEndDate').val());
        userData.append('ThemeId', $('#missionTheme').val());


        var inputName = "skillsToAdd";
        var inputId = "addSkillList";

        var inputTags = document.querySelectorAll('input[name="' + inputName + '"][id="' + inputId + '"]');

        inputTags.forEach(function (tag) {
            var value = parseInt(tag.value);
            if (!isNaN(value)) {
                userData.append('skillList', value);
                //integerValues.push(value);
            }
        });
        userData.append('TotalSeat', $('#missionSeat').val());
        userData.append('Deadline', $('#missionDeadline').val());
        userData.append('Availability', $('#missionAvailability').val());
        userData.append('CrudId', $('#missionId').val());
        userData.append('GoalValue', $('#goalValue').val());
        userData.append('GoalObjectiveText', $('#goalObject').val());
        var fileDoc = $('#missionDocument')[0].files;
        for (var i = 0; i < fileDoc.length; i++) {
            userData.append('missionDocuments', fileDoc[i]);
        }
        var videoUrls = document.getElementsByName('VideoUrl[]');
        for (var i = 0; i < videoUrls.length; i++) {
            userData.append('URL', videoUrls[i].value);
        }
        var fileImg = $('#missionImage')[0].files;
        for (var i = 0; i < fileImg.length; i++) {
            userData.append('file', fileImg[i]);
        }
        if ($('#missionTitle').val() == '') {
            $('#missionTitleError').text('Enter Title.');
            s1 = false;
        } else {
            $('#missionTitleError').text('');
        }
        if ($('#missionDescription').val() == '') {
            $('#missionDescriptionError').text('Enter Description.');
            s1 = false;
        } else {
            $('#missionDescriptionError').text('');
        }
        if ($('#missionShortDescription').val() == '') {
            $('#missionShortDescriptionError').text('Enter ShortDescription.');
            s1 = false;
        } else {
            $('#missionShortDescriptionError').text('');
        }
        if ($('#country').val() == 0) {
            $('#countryError').text('Select Country.');
            s1 = false;
        } else {
            $('#countryError').text('');
        } if ($('#selectCityList').val() == 0) {
            $('#selectCityListError').text('Select City.');
            s1 = false;
        } else {
            $('#selectCityListError').text('');
        }
        if ($('#missionOrganisationName').val() == '') {
            $('#missionOrganisationNameError').text('Enter OrganisationName.');
            s1 = false;
        } else {
            $('#missionOrganisationNameError').text('');
        }
        if ($('#missionOrganisationDetail').val() == '') {
            $('#missionOrganisationDetailError').text('Enter OrganisationDetail.');
            s1 = false;
        } else {
            $('#missionOrganisationDetailError').text('');
        }
        if ($('#missionStartDate').val() == '') {
            $('#missionStartDateError').text('Select Start Date.');
            s1 = false;
        } else {
            $('#missionStartDateError').text('');
        }
        if ($('#missionEndDate').val() == '') {
            $('#missionEndDateError').text('Select End Date.');
            s1 = false;
        } else {
            if ($('#missionStartDate').val() < $('#missionEndDate').val()) {
                $('#missionEndDateError').text('');
            } else {
                $('#missionEndDateError').text('Select EndDate after Start\'s Date ');
                s1 = false;
            }
        }

        if ($('#missionType').val() == "Time") {
            if ($('#missionSeat').val() == '') {
                $('#missionSeatError').text('Enter Total Seat No.');
                s1 = false;
            } else {
                if ($('#missionSeat').val() <= 0) {
                    $('#missionSeatError').text('Enter Positive No.');
                    s1 = false;
                }
                else {
                    $('#missionSeatError').text('');
                }
            }
            if ($('#missionDeadline').val() == '') {
                $('#missionDeadlineError').text('Select End Date.');
                s1 = false;
            } else {
                if ($('#missionStartDate').val() > $('#missionDeadline').val()) {
                    $('#missionDeadlineError').text('');
                } else {
                    $('#missionDeadlineError').text('Select Deadline before Start\'s Date ');
                    s1 = false;
                }
            }
        } else {
            if ($('#goalObject').val() == '') {
                $('#goalObjectError').text('Enter goal objective text');
                s1 = false;
            } else {
                $('#goalObjectError').text('');
            }
            if ($('#goalValue').val() == '') {
                $('#goalValueError').text('Enter Goal Value.');
                s1 = false;
            } else {
                if ($('#goalValue').val() <= 0) {
                    $('#goalValueError').text('Enter Positive No.');
                    s1 = false;
                }
                else {
                    $('#goalValueError').text('');
                }
            }
        }
        if ($('#missionAvailability').val() == 0) {
            $('#missionAvailabilityError').text('Select missionAvailability');
            s1 = false;
        } else {
            $('#missionAvailabilityError').text('');
        }
        if (fileDoc.length > 0) {
            $('#missionDocumentError').text('');
        } else {
            $('#missionDocumentError').text('Select atleast 1 Document.');
            s1 = false;
        }
        if (fileImg.length > 0) {
            $('#missionImageError').text('');
        } else {
            $('#missionImageError').text('Select atleast 1 Image.');
            s1 = false;
        }
    }
    if (type == 'theme') {
        userData.append('Title', $('#themeTitle').val());
        userData.append('Status', $('#statusTheme').val());
        userData.append('CrudId', $('#themeId').val());
        if ($('#themeTitle').val() == '') {
            $('#themeTitleError').text('Enter Title.');
            s1 = false;
        } else {
            $('#themeTitleError').text('');
        }
        if ($('#statusTheme').val() == 0) {
            $('#statusThemeError').text('Select Status.');
            s1 = false;
        } else {
            $('#statusThemeError').text('');
        }
    }
    if (type == 'skill') {
        userData.append('Title', $('#skillName').val());
        userData.append('Status', $('#statusSkill').val());
        userData.append('CrudId', $('#skillId').val());
        if ($('#skillName').val() == '') {
            $('#skillNameError').text('Enter SkillName.');
            s1 = false;
        } else {
            $('#skillNameError').text('');
        }
        if ($('#statusSkill').val() == 0) {
            $('#statusSkillError').text('Select Status.');
            s1 = false;
        } else {
            $('#statusSkillError').text('');
        }
    }
    if (type == 'banner') {
        userData.append('CrudId', $('#bannerId').val());
        userData.append('UserId', $('#sortOrder').val());
        userData.append('Description', $('#bannerText').val());
        var a = document.getElementById("bannerPhoto");;
        var b = a.files;
        for (var i = 0; i < b.length; i++) {
            userData.append('bannerPhoto', b[i]);
        }
        if ($('#sortOrder').val() == '') {
            $('#sortOrderError').text('Enter sortOrder No.');
            s1 = false;
        } else {
            var sortOrder = $('#sortOrder').val()
            if (sortOrder <= 0) {
                $('#sortOrderError').text('Enter Positive No.');
                s1 = false;
            }
            else {
                s1 = sameData;
            }
        }
        if ($('#bannerText').val() == 0) {
            $('#bannerTextError').text('Enter Banner Text.');
            s1 = false;
        } else {
            $('#bannerTextError').text('');
        }
        if (b.length > 0) {
            $('#bannerPhotoError').text('');
        } else {
            $('#bannerPhotoError').text('Select Banner Photo.');
            s1 = false;
        }
    }
    var Id = userData.get('CrudId');
    debugger;
    if (s1) {
        $.ajax({
            url: "/Admin/addViaAdmin",
            method: "POST",
            data: userData,
            processData: false,
            contentType: false,
            success: function (data) {
                debugger
                toastr.options = {
                    "positionClass": "toast-bottom-right"
                }
                if (Id == 0) {
                    toastr.success("Data added Successfully...");
                }
                else {
                    toastr.success("Data updated Successfully...");
                }
                if (type == "user") {
                    $('#addUser').modal('hide');
                    $('#editUser').modal('hide');
                    $('#users').html(data);
                }
                else if (type == "cms") {
                    $('#addCMS').modal('hide');
                    $('#cms').html(data);
                }
                else if (type == "mission") {
                    $('#addMission').modal('hide');
                    $('#missions').html(data);
                }
                else if (type == "theme") {
                    $('#addTheme').modal('hide');
                    $('#themes').html(data);
                }
                else if (type == "skill") {
                    $('#addSkill').modal('hide');
                    $('#skills').html(data);
                }
                else if (type == "banner") {
                    $('#addBanner').modal('hide');
                    $('#banneres').html(data);
                }
            },
            error: function (request, error) {
                console.log(error);
            }
        })
    }

}

function getViaAdmin(Id, type) {
    debugger

    $.ajax({
        url: "/Admin/getViaAdmin",
        method: "Post",
        data: {
            "Id": Id,
            "type": type
        },
        success: function (data) {
            debugger
            //data = JSON.parse(data);

            console.log(data);
            if (type == "user") {

                $('#getfirstName').val(data.firstName);
                $('#getlastName').val(data.lastName);
                $('#getemail').val(data.email);
                //$('#password').val(data);
                $('#getphoneNumber').val(data.phoneNumber);
                $('#getrole').val(data.role);
                $('#getemployeeId').val(data.employeeId);
                $('#getdepartment').val(data.department);
                $('#getcity').val(data.cityName);
                $('#getcountry').val(data.countryName);
                $('#statusUser').val(data.status);
                $('#getprofileText').val(data.profileText);
                $('#userId').val(data.userId);
                if (data.avatar != null) {
                    $('#old-profile-image').attr('src', "/Assets/" + data.avatar);
                }
                else {
                    $('#old-profile-image').attr('src', "/Assets/user1.png");
                }
            }
            if (type == "cms") {
                $('#title').val(data.title);
                $('#description').val(data.description);
                $('#slug').val(data.slug);
                $('#statusCms').val(data.status);
                $('#CmsPageId').val(data.cmsPageId);
            }
            if (type == "mission") {
                $('#missionId').val(data.missionId);
                $('#missionTitle').val(data.title);
                $('#missionDescription').val(data.description);
                $('#missionShortDescription').val(data.shortDescription);
                $('#country').val(data.countryId);
                $('#selectCityList').val(data.cityId);
                $('#missionType').val(data.missionType);
                $('#missionOrganisationName').val(data.organizationName);
                $('#missionOrganisationDetail').val(data.organizationDetail);
                var startDate = new Date(data.startDate);
                var endDate = new Date(data.endDate);
                var deadline = new Date(data.deadline);
                $('#missionStartDate').val(startDate.toISOString().split('T')[0]);
                $('#missionEndDate').val(endDate.toISOString().split('T')[0]);
                $('#missionTheme').val(data.themeId);

                //for (var i = 0; i < data.skillId.length; i++) {
                //    $('#missionSkill').val(data.skillId.skillId);

                //}
                data.skillId.forEach((name) => {
                    document.getElementById("addSkillsHere").innerHTML += `
 <li style="list-style-type:none">
                                                                <input type="hidden" value="${name.skillId}" />
                                                                ${name.skillName}
                                                                </li>

`;
                    document.getElementById("selected_features").innerHTML += `
<option value="${name.skillId}">${name.skillName}</option>
                                                               

`;
                })

                $("#features").empty();
                data.skill.forEach((name) => {
                    document.getElementById("features").innerHTML += `
<option value="${name.skillId}">${name.skillName}</option>


`;
                })



                $('#missionSeat').val(data.totalSeats);
                if (data.missionType != 'Time') {
                    $('#goalValue').val(data.goalValue[0].goalValue);
                    $('#goalObject').val(data.goalValue[0].goalObjectiveText);
                }
                $('#missionDeadline').val(deadline.toISOString().split('T')[0]);
                $('#missionAvailability').val(data.availability);
                misTypeData();
                getUserCity(data.countryId);

                //var dataTransfer1 = new DataTransfer();
                //var files1 = data.missionDocuments;
                //$('#missionDocument').each(function (i) {
                //    var file1 = new File(['boo'], files1[i].documentPath);
                //    dataTransfer1.items.add(file1);
                //    this.files = dataTransfer1.files;
                //});
                //for (let i = 0; i < files1.length; i++) {
                //    var file1 = new File(['foo'], files1[i].documentPath);
                //    dataTransfer1.items.add(file1);
                //    $('#missionDocument').files = dataTransfer1.files;
                //}
                var dataTransfer1 = new DataTransfer();
                var files1 = data.missionDocuments;
                $('#missionDocument').each(function (i) {
                    if (files1[i]) {
                        for (var j = 0; j < files1.length; j++) {
                            var file = new File(['foo'], files1[j].documentPath);
                            dataTransfer1.items.add(file);
                        }
                        this.files = dataTransfer1.files;
                    }
                });
                var dataTransfer = new DataTransfer();
                var files = data.missionMedia;
                $('#missionImage').each(function (i) {
                    if (files[i]) {
                        for (var j = 0; j < files.length; j++) {
                            var file = new File(['foo'], files[j].mediaPath);
                            dataTransfer.items.add(file);
                        }
                        this.files = dataTransfer.files;
                    }
                });
                // $('#missionDocument')[0].files;
                // $('#missionVideo')[0].files;
                // $('#missionImage')[0].files;
            }
            if (type == "theme") {
                $('#themeId').val(data.missionThemeId);
                $('#themeTitle').val(data.title);
                $('#statusTheme').val(data.status);
            }
            if (type == "skill") {
                $('#skillId').val(data.skillId);
                $('#skillName').val(data.skillName);
                $('#statusSkill').val(data.status);
            }
            if (type == "application") {
                $('#missionApplicationId').val(data.missionApplicationId);
            }
            if (type == "story") {
                $('#storyId').val(data.storyId);
            }
            if (type == "banner") {
                $('#bannerId').val(data.bannerId);
                $('#sortOrder').val(data.sortOrder);
                $('#bannerText').val(data.text);
                // Create a new DataTransfer object
                const dataTransfer = new DataTransfer();

                // Add a file to the DataTransfer object
                const file = new File(['foo'], data.image, { type: 'image/png' });
                dataTransfer.items.add(file);
                $('#bannerPhoto')[0].files = dataTransfer.files;

            }

        },
        error: function (request, error) {
            console.log(error);
        }
    })

}

function deleteViaAdmin(type) {

    debugger
    var Id = $('#userId').val();
    if (Id == '') {
        Id = $('#CmsPageId').val();
    }
    if (Id == '') {
        Id = $('#missionId').val();
    }
    if (Id == '') {
        Id = $('#themeId').val();
    }
    if (Id == '') {
        Id = $('#skillId').val();
    }
    if (Id == '') {
        Id = $('#missionApplicationId').val();
    }
    if (Id == '') {
        Id = $('#storyId').val();
    } if (Id == '') {
        Id = $('#bannerId').val();
    }
    $.ajax({
        url: '/Admin/addViaAdmin',
        type: 'POST',
        data: {
            Id: Id,
            type: type
        },
        success: function (data) {
            debugger

            console.log(data);

            toastr.options = {
                "positionClass": "toast-bottom-right"
            }
            if (type == "user") {
                $('#deleteUser').modal('hide');
                $('#users').html(data);
                toastr.success("User Deleted Successfully...");
            }
            else if (type == "cms") {
                $('#deleteCMS').modal('hide');
                $('#cms').html(data);
                toastr.success("CMS Deleted Successfully...");
            }
            else if (type == "mission") {
                $('#deleteMission').modal('hide');
                $('#missions').html(data);
                toastr.success("Mission Deleted Successfully...");
            }
            else if (type == "theme") {
                $('#deleteTheme').modal('hide');
                $('#themes').html(data);
                toastr.success("Theme Deleted Successfully...");
            }
            else if (type == "skill") {
                $('#deleteSkill').modal('hide');
                $('#skills').html(data);
                toastr.success("Skill Deleted Successfully...");

            }
            else if (type == "approveApplication") {
                $('#approveApplication').modal('hide');
                $('#applications').html(data);
                toastr.success("Mission Approved Successfully...");

            }
            else if (type == "declineApplication") {
                $('#declineApplication').modal('hide');
                $('#applications').html(data);
                toastr.success("Mission Declined...");

            }
            else if (type == "approveStory") {
                $('#approveStory').modal('hide');
                $('#stories').html(data);
                toastr.success("Story Approved Successfully...");

            }
            else if (type == "declineStory") {
                $('#declineStory').modal('hide');
                $('#stories').html(data);
                toastr.success("Story Declined...");

            }
            else if (type == "deleteStory") {
                $('#deleteStory').modal('hide');
                $('#stories').html(data);
                toastr.success("Story Deleted Successfully...");

            }
            else if (type == "banner") {
                $('#deleteBanner').modal('hide');
                $('#banneres').html(data);
                toastr.success("Banner Deleted Successfully...");

            }
        },
        error: function (error) {
            alert('An error occurred while sending data: ' + error);
        }
    });

}

function misTypeData() {
    debugger
    var value = $('#missionType').val();
    if (value == "Goal") {
        $('.timemissionAdmin').addClass("d-none");
        $('.goalmissionAdmin').removeClass("d-none");
    } else {
        $('.timemissionAdmin').removeClass("d-none");
        $('.goalmissionAdmin').addClass("d-none");
    }
}

function clearModalAdmin() {

    var elementIds = ['#firstName', '#lastName', '#email', '#password', '#phoneNumber', '#title', '#description', '#slug', '#themeTitle', '#skillName', '#bannerText', '#sortOrder', '#bannerPhoto'];
    for (var i = 0; i < elementIds.length; i++) {
        $(elementIds[i]).val('');
    }

    $('#statusCms').val(0);
    $('#statusTheme').val(0);
    $('#statusSkill').val(0);
    var elementIds = ["#addUser", "#addCMS", "#addTheme", "#addSkill", "#addBanner", "#addMission"];
    for (var j = 0; j < elementIds.length; j++) {
        var spanElements = document.querySelectorAll(elementIds[j] + " span");
        for (var i = 0; i < spanElements.length; i++) {
            spanElements[i].textContent = "";
        }
    }

    var inputElements = document.querySelectorAll("#addMission input, #addMission textarea");
    for (var i = 0; i < inputElements.length; i++) {
        var elementType = inputElements[i].type.toLowerCase();
        if (elementType === "text" || elementType === "password" || elementType === "email" || elementType === "date" || inputElements[i].tagName.toLowerCase() === "textarea") {
            inputElements[i].value = "";
        } else if (elementType === "checkbox" || elementType === "radio") {
            inputElements[i].checked = false;
        }
    }

}

function UpdateSetting() {
debugger
var settingList = [];

$('.setting:checked').each(function () {
    settingList.push(this.value);
});
$('#notification-setting').modal('hide');
$.ajax({
    url: "/User/UpdateSetting",
    method: "POST",
    data: { "settingList": settingList },
    success: function (data) {
        if (data) {
            toastr.options = {
                "positionClass": "toast-bottom-right"
            }
            toastr.success("Settings Update Successfully...")
        }
    },
    error: function (request, error) {
        console.log(error);
    }
})
}

function ReadNotification(NotificationId) {
    debugger
    $.ajax({
        url: "/User/ReadNotification",
        method: "POST",
        data: { "NotificationId": NotificationId },
        success: function (data) {
        },
        error: function (request, error) {
            console.log(error);
        }
    })
}