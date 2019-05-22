//[Preview Menu Javascript]

//Project:	Fab Admin - Responsive Admin Template
//Primary use:   This file is for demo purposes only.

$(function () {
  'use strict'

  /**
   * Get access to plugins
   */

  $('[data-toggle="control-sidebar"]').controlSidebar()
  $('[data-toggle="push-menu"]').pushMenu()

  var $pushMenu       = $('[data-toggle="push-menu"]').data('lte.pushmenu')
  var $controlSidebar = $('[data-toggle="control-sidebar"]').data('lte.controlsidebar')
  var $layout         = $('body').data('lte.layout')

  /**
   * List of all the available skins
   *
   * @type Array
   */
  var mySkins = [
    'skin-pink',
    'skin-blue',
    'skin-red',
    'skin-yellow',
    'skin-purple',
    'skin-green',
    //'skin-pink-light',
    //'skin-black-light',
    //'skin-red-light',
    //'skin-yellow-light',
    //'skin-purple-light',
    //'skin-green-light'
  ]

  /**
   * Get a prestored setting
   *
   * @param String name Name of of the setting
   * @returns String The value of the setting | null
   */
  function get(name) {
    if (typeof (Storage) !== 'undefined') {
      return localStorage.getItem(name)
    } else {
      window.alert('Please use a modern browser to properly view this template!')
    }
  }

  /**
   * Store a new settings in the browser
   *
   * @param String name Name of the setting
   * @param String val Value of the setting
   * @returns void
   */
  function store(name, val) {
    if (typeof (Storage) !== 'undefined') {
      localStorage.setItem(name, val)
    } else {
      window.alert('Please use a modern browser to properly view this template!')
    }
  }

  /**
   * Toggles layout classes
   *
   * @param String cls the layout class to toggle
   * @returns void
   */
  function changeLayout(cls) {
    $('body').toggleClass(cls)
    $layout.fixSidebar()
    if ($('body').hasClass('fixed') && cls == 'fixed') {
      $pushMenu.expandOnHover()
      $layout.activate()
    }
    $controlSidebar.fix()
  }

  /**
   * Replaces the old skin with the new skin
   * @param String cls the new skin class
   * @returns Boolean false to prevent link's default action
   */
  function changeSkin(cls) {
    $.each(mySkins, function (i) {
      $('body').removeClass(mySkins[i])
    })

    $('body').addClass(cls)
    store('skin', cls)
    return false
  }

  /**
   * Retrieve default settings and apply them to the template
   *
   * @returns void
   */
  function setup() {
    var tmp = get('skin')
    if (tmp && $.inArray(tmp, mySkins))
      changeSkin(tmp)

    // Add the change skin listener
    $('[data-skin]').on('click', function (e) {
      if ($(this).hasClass('knob'))
        return
      e.preventDefault()
      changeSkin($(this).data('skin'))
    })

    // Add the layout manager
    $('[data-layout]').on('click', function () {
      changeLayout($(this).data('layout'))
    })

    $('[data-controlsidebar]').on('click', function () {
      changeLayout($(this).data('controlsidebar'))
      var slide = !$controlSidebar.options.slide

      $controlSidebar.options.slide = slide
      if (!slide)
        $('.control-sidebar').removeClass('control-sidebar-open')
    })

    $('[data-sidebarskin="toggle"]').on('click', function () {
      var $sidebar = $('.control-sidebar')
      if ($sidebar.hasClass('control-sidebar-dark')) {
        $sidebar.removeClass('control-sidebar-dark')
        $sidebar.addClass('control-sidebar-light')
      } else {
        $sidebar.removeClass('control-sidebar-light')
        $sidebar.addClass('control-sidebar-dark')
      }
    })

    $('[data-enable="expandOnHover"]').on('click', function () {
      $(this).attr('disabled', true)
      $pushMenu.expandOnHover()
      if (!$('body').hasClass('sidebar-collapse'))
        $('[data-layout="sidebar-collapse"]').click()
    })

    //  Reset options
    if ($('body').hasClass('fixed')) {
      $('[data-layout="fixed"]').attr('checked', 'checked')
    }
    if ($('body').hasClass('layout-boxed')) {
      $('[data-layout="layout-boxed"]').attr('checked', 'checked')
    }
    if ($('body').hasClass('sidebar-collapse')) {
      $('[data-layout="sidebar-collapse"]').attr('checked', 'checked')
    }

  }

  // Create the new tab
  var $tabPane = $('<div />', {
    'id'   : 'control-sidebar-theme-demo-options-tab',
    'class': 'tab-pane active'
  })

  // Create the tab button
  var $tabButton = $('<li />', { 'class': 'nav-item' })
    .html('<a href=\'#control-sidebar-theme-demo-options-tab\' class=\'active\' data-toggle=\'tab\'>'
      + '<i class="fa fa-wrench"></i>'
      + '</a>')

  // Add the tab button to the right sidebar tabs
  $('[href="#control-sidebar-home-tab"]')
    .parent()
    .before($tabButton)

  // Create the menu
  var $demoSettings = $('<div />')

  var $skinsList = $('<ul />', { 'class': 'list-unstyled clearfix' })

  // Dark sidebar skins
  var $skinPink =
        $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
          .append('<a href="javascript:void(0)" data-skin="skin-pink" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
            + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #242a33; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-pink" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
            + '</a>'
            + '<p class="text-center no-margin">Pink</p>')
  $skinsList.append($skinPink)
  var $skinBlue =
        $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
          .append('<a href="javascript:void(0)" data-skin="skin-blue" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
            + '<div class="clearfix"><span style="display:block; width: 40%; float: left; height: 40px; background: #242a33; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span style="display:block; width: 60%; float: left; height: 40px; background: #3593fa; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
            + '</a>'
            + '<p class="text-center no-margin">Blue</p>')
  $skinsList.append($skinBlue)
  var $skinPurple =
        $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
          .append('<a href="javascript:void(0)" data-skin="skin-purple" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
            + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #242a33; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-purple" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
            + '</a>'
            + '<p class="text-center no-margin">Purple</p>')
  $skinsList.append($skinPurple)
  var $skinGreen =
        $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
          .append('<a href="javascript:void(0)" data-skin="skin-green" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
            + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #242a33; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-success" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
            + '</a>'
            + '<p class="text-center no-margin">Green</p>')
  $skinsList.append($skinGreen)
  var $skinRed =
        $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
          .append('<a href="javascript:void(0)" data-skin="skin-red" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
            + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #242a33; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-red" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
            + '</a>'
            + '<p class="text-center no-margin">Red</p>')
  $skinsList.append($skinRed)
  var $skinYellow =
        $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
          .append('<a href="javascript:void(0)" data-skin="skin-yellow" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
            + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #242a33; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-yellow" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
            + '</a>'
            + '<p class="text-center no-margin">Yellow</p>')
  $skinsList.append($skinYellow)

  // Light sidebar skins
  //var $skinBlueLight =
  //      $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
  //        .append('<a href="javascript:void(0)" data-skin="skin-pink-light" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
  //          + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #ffffff; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-blue" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
  //          + '</a>'
  //          + '<p class="text-center no-margin" style="font-size: 12px">Blue Light</p>')
  //$skinsList.append($skinBlueLight)
  //var $skinBlackLight =
  //      $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
  //        .append('<a href="javascript:void(0)" data-skin="skin-black-light" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
  //          + '<div class="clearfix"><span style="display:block; width: 40%; float: left; height: 40px; background: #ffffff; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span style="display:block; width: 60%; float: left; height: 40px; background: #2A3E52;  border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
  //          + '</a>'
  //          + '<p class="text-center no-margin" style="font-size: 12px">Black Light</p>')
  //$skinsList.append($skinBlackLight)
  //var $skinPurpleLight =
  //      $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
  //        .append('<a href="javascript:void(0)" data-skin="skin-purple-light" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
  //          + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #ffffff; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-purple" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
  //          + '</a>'
  //          + '<p class="text-center no-margin" style="font-size: 12px">Purple Light</p>')
  //$skinsList.append($skinPurpleLight)
  //var $skinGreenLight =
  //      $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
  //        .append('<a href="javascript:void(0)" data-skin="skin-green-light" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
  //          + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #ffffff; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-success" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
  //          + '</a>'
  //          + '<p class="text-center no-margin" style="font-size: 12px">Green Light</p>')
  //$skinsList.append($skinGreenLight)
  //var $skinRedLight =
  //      $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
  //        .append('<a href="javascript:void(0)" data-skin="skin-red-light" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
  //          + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #ffffff; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-red" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
  //          + '</a>'
  //          + '<p class="text-center no-margin" style="font-size: 12px">Red Light</p>')
  //$skinsList.append($skinRedLight)
  //var $skinYellowLight =
  //      $('<li />', { style: 'float:left; width: 33.33333%; padding: 5px;' })
  //        .append('<a href="javascript:void(0)" data-skin="skin-yellow-light" style="display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4);border-radius: 100px;" class="clearfix full-opacity-hover">'
  //          + '<div><span style="display:block; width: 40%; float: left; height: 40px; background: #ffffff; border-top-left-radius: 30px;border-bottom-left-radius: 30px;"></span><span class="bg-yellow" style="display:block; width: 60%; float: left; height: 40px; border-top-right-radius: 30px;border-bottom-right-radius: 30px;"></span></div>'
  //          + '</a>'
  //          + '<p class="text-center no-margin" style="font-size: 12px">Yellow Light</p>')
  //$skinsList.append($skinYellowLight)

  

  $demoSettings.append('<h4 class="control-sidebar-heading">Skins</h4>')
  $demoSettings.append($skinsList)

  $tabPane.append($demoSettings)
  $('#control-sidebar-home-tab').after($tabPane)

  setup()

  $('[data-toggle="tooltip"]').tooltip()
});// End of use strict
