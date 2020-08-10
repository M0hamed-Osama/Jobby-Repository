/*global window */
/*global document */

//self invoke function
(function(){
    // html dom access
    var myNavi = document.getElementById("mynav");
    var introText = document.getElementById("intro-txt");

    // calling functions on events  
    navbarBackground();
    window.onscroll = navbarBackground;
    window.addEventListener('resize', navbarBackground);
  
  // navigation bar background adjusting function
    function navbarBackground() {
        if (window.scrollY >= introText.offsetTop - myNavi.offsetHeight) {
            myNavi.style.backgroundColor = "#1c2540";
        }
        else {
            myNavi.style.backgroundColor = "rgba(0, 0, 0,0)";
        }   
  }
})();