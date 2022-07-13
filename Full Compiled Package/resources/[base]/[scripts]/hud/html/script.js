$(function () {
    let height = 25.5
    window.addEventListener('message', function (event) {
        if (event.data.type == "updateStatusHud") {
            $("#varSetHealth").find(".progressBar").attr("style", "width: "+event.data.varSetHealth + "%;")
            $("#varSetArmor").find(".progressBar").attr("style", "width: "+event.data.varSetArmor + "%;")

            widthHeightSplit(event.data.varSetHunger, $("#varSetHunger").find(".progressBar"))
            widthHeightSplit(event.data.varSetThirst, $("#varSetThirst").find(".progressBar"))
            widthHeightSplit(event.data.varSetOxy, $("#varSetOxy").find(".progressBar"))
            widthHeightSplit(event.data.varSetStress, $("#varSetStress").find(".progressBar"))

            let voice = event.data.varSetVoice
            // $(".voice1").addClass("hidden")
            // $(".voice2").addClass("hidden")
            // $(".voice3").addClass("hidden")
            $(".dev").addClass("hidden")
            $(".devDebug").addClass("hidden")
            if (event.data.talking == true) {
                $('#rect1').css('fill', '#ff1f69')
                $('#rect2').css('fill', '#ff1f69')
                $('#rect3').css('fill', '#ff1f69')
            } else {
                $('#rect1').css('fill', '#FFFFFF')
                $('#rect2').css('fill', '#FFFFFF')
                $('#rect3').css('fill', '#FFFFFF')
            }


            if (event.data.state == 0) {
                $('#rect1').css('visibility', 'hidden')
                $('#rect2').css('visibility', 'hidden')
                $('#rect3').css('visibility', 'hidden')
            } else if (event.data.state == 1){
                $('#rect1').css('visibility', 'hidden')
                $('#rect2').css('visibility', 'visible')
                $('#rect3').css('visibility', 'visible')
            } else if (event.data.state == 2){
                $('#rect1').css('visibility', 'hidden')
                $('#rect2').css('visibility', 'hidden')
                $('#rect3').css('visibility', 'visible')
            } else if (event.data.state == 3){
                $('#rect1').css('visibility', 'visible')
                $('#rect2').css('visibility', 'visible')
                $('#rect3').css('visibility', 'visible')
            }
            if (event.data.varDev == true)
            {
                $(".dev").removeClass("hidden")
            }
            if (event.data.varDevDebug == true)
            {
                $(".devDebug").removeClass("hidden")
            }
            if (event.data.hasParachute == true)
            {
                $("#parachute").removeClass("hidden")
            } else {
                $("#parachute").addClass("hidden")
            }


            

            changeColor($("#varSetHealth"), event.data.varSetHealth, false)
            changeColor($("#varSetArmor"), event.data.varSetArmor, false)
            changeColor($("#varSetHunger"), event.data.varSetHunger, false)
            changeColor($("#varSetThirst"), event.data.varSetThirst, false)
            changeColor($("#varSetOxy"), event.data.varSetOxy, false)
            changeColor($("#varSetStress"), event.data.varSetStress, true)

            if (event.data.varSetArmor <= 0) {
                $("#varSetArmor").find(".barIcon").removeClass("danger")
            }

            if (event.data.colorblind === true) {
                $(".progressBar").addClass("colorBlind")
            }
            else {
                $(".progressBar").removeClass("colorBlind")
            }
        }
    })

    function widthHeightSplit(value, ele) {
        let eleHeight = (value / 100) * height;
        let leftOverHeight = height - eleHeight;

        ele.attr("style", "height: "+eleHeight+"px; top: "+leftOverHeight+"px;")
    }

    function changeColor(ele, value, flip) {
        let add = false
        if (flip) {
            if (value > 85) {
                add = true
            }
        }
        else {
            if (value < 25) {
                add = true
            }
        }

        if (add) {
            // ele.find(".barIcon").addClass("danger")
            ele.find(".progressBar").addClass("dangerGrad")
        }
        else {
            // ele.find(".barIcon").removeClass("danger")
            ele.find(".progressBar").removeClass("dangerGrad")
        }
    }
})