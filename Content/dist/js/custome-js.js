//folder file etc.. hover effect
$(".headerMenu-icon").hover(
  function () {
      $('.headerBox-a-menu').slideDown('fast');
  },
  );
  
  $(".headerBox-a-menu").mouseleave(
  function () {
      $('.headerBox-a-menu').slideUp('fast');
  }
  );