using Base;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using Network;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CreateCharacter;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;

namespace AllMaps.ItemProvider
{
    public abstract class SkillProvider : ItemProvider<BaseSkill>
    {
        public override int BaseValueLevelWithTier => 3;

        protected void SetTextKeyword(Image prefabKeyword, int i, string content)
        {
            Image itemInfor = GameManager.GetItemPoolWithPrefabInParent<Image>(prefabKeyword);

            itemInfor.gameObject.SetActive(true);
            itemInfor.transform.GetChild(0).GetComponent<Text>().text = $"{content}";
        }

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
            if (ctx.BaseItemData is BaseSkill skill)
            {
                ctx.QualityText.text = BaseSkill.GetTextTierSkill(skill.eTierSkill);
                ctx.TypeText.text = skill.GetTextTypeSkill();


                var listEKeyword = skill.GetEKeywordSkillHave();
                var listEBuffKeyword = skill.GetEKeywordBuffSkillHave(1);
                for (int i = 0; i < listEKeyword.Length; i++)
                {
                    SetTextKeyword(ctx.PrefabLeftInfor, i, BaseSkill.GetDescKeyWord(listEKeyword[i]));
                }

                for (int i = 0; i < listEBuffKeyword.Length; i++)
                {
                    SetTextKeyword(ctx.PrefabRightInfor, i, BaseSkill.GetDescBuffOrDeBuff(listEBuffKeyword[i]));
                }
            }
        }

        public override int GetPriceItemSell(ushort Id)
        {
            var data = GetDataItemByIdRef(Id);
            if (data)
            {
                return GameManager.PriceTTWithTier[data.eTierSkill];
            }

            return 0;
        }
    }

    public abstract class AbstractThanThongProvider : SkillProvider
    {
        public override ItemType SupportedType => ItemType.ThanThong;
        public abstract string TypeText { get; }

        protected virtual List<ThanThong.RequireActive> GetRequireActive(ThanThong skill)
        {
            return skill.BaseRequireActive;
        }

        protected virtual string GetDescThanThong(ThanThong skill, int level)
        {
            return skill.GetDesc(ThanThong.GetDaiCanhGioi(level));
        }

        protected void DisplayBaseThanThong(ToolTipContext ctx, ThanThong skill,
            List<ThanThong.RequireActive> requireActives, string desc)
        {
            var parent = ctx.PrefabIconRequire.transform.parent.parent;
            parent.gameObject.SetActive(true);

            for (int i = 0; i < requireActives.Count; i++)
            {
                Image itemRequire = GameManager.GetItemPoolWithPrefabInParent(ctx.PrefabIconRequire);

                itemRequire.gameObject.SetActive(true);
                itemRequire.sprite = ctx.SpriteLinhKhis[(int)requireActives[i].linhKhiRequire];
                itemRequire.SetNativeSize();
                itemRequire.transform.GetChild(0).GetComponent<Text>().text = $"x{requireActives[i].count}";
            }

            ctx.DescriptionText.text = desc;
        }

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
            ctx.TypeText.text += $" [{TypeText}]";
            var charData = Client.Instance.CharacterData;
            if (ctx.BaseItemData is ThanThong skill)
            {
                var requireActives = GetRequireActive(skill);
                DisplayBaseThanThong(ctx, skill, requireActives, GetDescThanThong(skill, charData.Progression.Level));
            }
        }

        public override BaseSkill GetDataItemByIdRef(ushort Id)
        {
            return Pool.Get<GameManager>().GetThanThongWithId(Id);
        }
    }

    public class ThanThongProviderCombat : AbstractThanThongProvider
    {
        public override string TypeText => "Thần Thông";

        protected List<ThanThong.RequireActive> GetRequireActiveCombat(ThanThong skill, CharacterCombat charCombat)
        {
            return skill.GetRequireActive(charCombat);
        }

        protected string GetDescThanThongCombat(ThanThong skill, CharacterCombat charCombat)
        {
            var daiCanhGioi = ThanThong.GetDaiCanhGioi(charCombat.characterStatsBase.level);
            return skill is TranPhapTT tranPhapTT
                ? tranPhapTT.GetDescTranPhap(charCombat, daiCanhGioi)
                : skill.GetDesc(daiCanhGioi);
        }

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
            ctx.PriceText.transform.parent.gameObject.SetActive(false);
            if (ctx.BaseItemData is BaseSkill skill)
            {
                ctx.TypeText.text = skill.GetTextTypeSkill();
            }
        }

        public void DisplayCombat(ToolTipContext ctx, CharacterCombat charCombat)
        {
            Display(ctx);
            if (ctx.BaseItemData is ThanThong skill)
            {
                var requireActives = GetRequireActiveCombat(skill, charCombat);
                DisplayBaseThanThong(ctx, skill, requireActives, GetDescThanThongCombat(skill, charCombat));
            }
        }
    }

    public class ThanThongBookProvider : AbstractThanThongProvider, IUseItemProvider
    {
        public override string TypeText => "Sách TT";

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
            ctx.PriceText.transform.parent.gameObject.SetActive(true);
        }

        public void UseItem(ItemInventory itemInventory)
        {
            NotifyGroup.Instance?.Show("Vui lòng Lĩnh ngộ tại mục bế quan trong khách sạn!");
        }
    }

    public class ThanThongProvider : AbstractThanThongProvider
    {
        public override string TypeText => "TT";

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
            ctx.PriceText.transform.parent.gameObject.SetActive(false);
        }
    }

    public abstract class AbstractCongPhapProvider : SkillProvider
    {
        public override ItemType SupportedType => ItemType.CongPhap;
        public abstract string TypeText { get; }

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
            ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
            ctx.TypeText.text += $" [{TypeText}]";
            if (ctx.BaseItemData is CongPhap skill)
            {
                ctx.PrefabIconRequire.transform.parent.parent.gameObject.SetActive(false);
                SetDisplay(ctx, skill);
            }
        }

        public abstract void SetDisplay(ToolTipContext ctx, CongPhap cp);

        public override BaseSkill GetDataItemByIdRef(ushort Id)
        {
            return Pool.Get<GameManager>().GetCongPhapWithId(Id);
        }
    }

    public class CongPhapBookProvider : AbstractCongPhapProvider, IUseItemProvider
    {
        public override string TypeText => "Sách CP";

        public override void SetDisplay(ToolTipContext ctx, CongPhap cp)
        {
            ctx.DescriptionText.text = cp.GetDesc(5);
            ctx.DescTextItem.text = $"<color=#ffb143>【Tốc độ tu luyện】</color> {cp.GetTocDoTuLuyen(5)}";
            ctx.PriceText.transform.parent.gameObject.SetActive(true);
        }

        public void UseItem(ItemInventory itemInventory)
        {
            NotifyGroup.Instance?.Show("Vui lòng Lĩnh ngộ tại mục bế quan trong khách sạn!");
        }
    }

    public class CongPhapProvider : CongPhapBookProvider
    {
        public override string TypeText => "CP";

        public override void SetDisplay(ToolTipContext ctx, CongPhap cp)
        {
            ctx.PriceText.transform.parent.gameObject.SetActive(false);
            var charData = Client.Instance.CharacterData;
            var cpChar = charData.InventoryManager.CongPhapOwners.FirstOrDefault(x => x.Id == cp.Id);
            if (cpChar != null)
            {
                ctx.DescriptionText.text = cp.GetDesc(cpChar.Level);
                ctx.DescTextItem.text =
                    $"<color=#ffb143>【Tiến độ】</color> Tầng thứ {cpChar.Level}   <color=#ffb143>【Tốc độ tu luyện】</color> {cp.GetTocDoTuLuyen(cpChar.Level)}";
            }
            else
            {
                base.SetDisplay(ctx, cp);
            }
        }
    }
}