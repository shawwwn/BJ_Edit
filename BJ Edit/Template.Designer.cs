﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.269
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BJ_Edit {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Template {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Template() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BJ_Edit.Template", typeof(Template).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 else
        ///                    // indexRace is a integer converted from real, so automatically add 1
        ///                    set indexRace = indexRace + 1
        ///                endif
        ///            else
        ///                set indexRace=0
        ///            endif
        ///            
        ///            set MC_raceindex[index] = indexRace
        ///
        ///            // debug uses.
        ///            //call BJDebugMsg( ( &quot;Player&quot;+I2S(index)+&quot; RaceIndex:&quot; + I2S(indexRace) ) )
        ///
        ///            // Create initial race-specific starting units 的本地化字符串。
        /// </summary>
        internal static string body {
            get {
                return ResourceManager.GetString("body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 if (indexRace == MC_RACE_[RACE]) then
        ///                    call PickMeleeAI(indexPlayer, &quot;[race].ai&quot;, null, null)
        ///                else 的本地化字符串。
        /// </summary>
        internal static string body2 {
            get {
                return ResourceManager.GetString("body2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 else
        ///                call MeleeStartingUnitsUnknownRace(indexPlayer, indexStartLoc, true, true, true)
        ///            endif
        ///        endif
        ///     call SetPlayerHandicap(indexPlayer, 1.00)
        ///     set index = index + 1
        ///     exitwhen index == bj_MAX_PLAYERS
        ///endloop
        ///endfunction 的本地化字符串。
        /// </summary>
        internal static string feet {
            get {
                return ResourceManager.GetString("feet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 // Unrecognized race.
        ///                endif
        ///                call ShareEverythingWithTeamAI(indexPlayer)
        ///            endif
        ///        endif
        ///
        ///        set index = index + 1
        ///        exitwhen index == bj_MAX_PLAYERS
        ///    endloop
        ///endfunction 的本地化字符串。
        /// </summary>
        internal static string feet2 {
            get {
                return ResourceManager.GetString("feet2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 function MeleeStartingUnits takes nothing returns nothing
        ///local integer  index
        ///local player   indexPlayer
        ///local location indexStartLoc
        ///local integer  indexRace
        ///
        ///set index = 0
        ///loop
        ///     set indexPlayer = Player(index)
        ///        if (GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING) then
        ///            set indexStartLoc = GetStartLocationLoc(GetPlayerStartLocation(indexPlayer))
        ///            
        ///            if (GetPlayerController(indexPlayer) == MAP_CONTROL_USER) then
        ///                set indexR [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string head {
            get {
                return ResourceManager.GetString("head", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 function MeleeStartingAI takes nothing returns nothing
        ///    local integer index
        ///    local player  indexPlayer
        ///    local integer indexRace
        ///
        ///    set index = 0
        ///    loop
        ///        set indexPlayer = Player(index)
        ///        if (GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING) then
        ///            set indexRace = MC_raceindex[index]
        ///            if (GetPlayerController(indexPlayer) == MAP_CONTROL_COMPUTER) then
        ///                // Run a race-specific melee AI script. 的本地化字符串。
        /// </summary>
        internal static string head2 {
            get {
                return ResourceManager.GetString("head2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 call MC_InitRaceIndexGC()
        ///call MC_SyncRaceIndexForAll() 的本地化字符串。
        /// </summary>
        internal static string rigc_callfuncs {
            get {
                return ResourceManager.GetString("rigc_callfuncs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 //===========================================================================
        ///// Starting Units for [RaceName] Players
        /////   - 1 [baseID], placed at start location
        /////   - 5 [peonID], placed between start location and nearest gold mine
        /////
        ///// MC_EDITED 的本地化字符串。
        /// </summary>
        internal static string startingunitcomment {
            get {
                return ResourceManager.GetString("startingunitcomment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 function [FuncName] takes player whichPlayer, location startLoc, boolean doHeroes, boolean doCamera, boolean doPreload returns nothing
        ///    local boolean  useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO)
        ///    local real     unitSpacing   = 64.00
        ///    local unit     nearestMine
        ///    local location nearMineLoc
        ///    local location heroLoc
        ///    local real     peonX
        ///    local real     peonY
        ///
        ///    set nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS)
        ///    if (nearestMine != null) then
        ///     [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string startingunittemplate {
            get {
                return ResourceManager.GetString("startingunittemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 //&lt;----- Sync Gamecache
        ///function MC_InitRaceIndexGC takes nothing returns nothing
        ///    if(MC_udg_raceindexgc == null)then
        ///        call FlushGameCache(InitGameCache(&quot;RaceIndexGC.w3v&quot;))
        ///        set MC_udg_raceindexgc = InitGameCache(&quot;RaceIndexGC.w3v&quot;)
        ///    endif
        ///endfunction
        ///
        ///function MC_SyncRaceIndexForPlayer takes player p returns boolean
        ///    local integer x
        ///    local string k1
        ///    local string k2
        ///    if(GetPlayerController(p) != MAP_CONTROL_USER or GetPlayerSlotState(p)==PLAYER_SLOT_STATE_EMPTY)th [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string sync_body {
            get {
                return ResourceManager.GetString("sync_body", resourceCulture);
            }
        }
    }
}
