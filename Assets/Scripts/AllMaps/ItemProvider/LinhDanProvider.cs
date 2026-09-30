using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using System;
using System.Linq;

namespace AllMaps.ItemProvider
{
    public class LinhDanProvider : ItemProvider<LinhDan>, IUseItemProvider
    {
        public override int BaseValueLevelWithTier => 6;
        public override ItemType SupportedType => ItemType.LinhDan;

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);

            string text = "Đan Dược";

            if (ctx.IItemData is LinhDan linhDan)
            {
                ctx.DescTextItem.transform.parent.gameObject.SetActive(true);

                switch (linhDan.ETypeDan)
                {
                    case ETypeDan.DanCombat: text += " - Chiến Đấu"; break;
                    case ETypeDan.DanBuff: text += " - Tăng Chỉ Số"; break;
                    case ETypeDan.DanDotPha: text += " - Đột Phá"; break;
                    case ETypeDan.DanOther: text += " - Khác"; break;
                }

                ctx.DescTextItem.text = $"{BaseSkill.GetColoredText("【Đan Độc】", "#d36cd3")} {linhDan.DanDoc}";
                ctx.DescriptionText.text = linhDan.Description;
            }

            ctx.TypeText.text = text;
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }

        public override LinhDan GetDataItemByIdRef(ushort Id)
        {
            return LuyenDanResources.Instance.FindLinhDan(Id);
        }

        public override int GetPriceItemSell(ushort Id)
        {
            var data = GetDataItemByIdRef(Id);
            if (data != null && data is LinhDan linhDan)
            {
                return linhDan.PriceV130;
            }

            return 0;
        }

        public override int GetCountOwner(ushort id)
        {
            var item = Client.Instance.CharacterData.InventoryManager.LinhDans.FirstOrDefault(x => x.Id == id);
            return item != null ? item.Count : base.GetCountOwner(id);
        }

        public void UseItem(ItemInventory itemInventory)
        {
            if (itemInventory.iItemData is LinhDan danDuoc)
            {
                // Đang dùng đan cho một đích khác (vd. Rối trong Tông môn chiến).
                var target = DanUseRouter.Current;
                if (target != null && !target.CanUse(danDuoc, out var reason))
                {
                    NotifyGroup.Instance.Show(reason);
                    return;
                }

                if (danDuoc.ETypeDan == ETypeDan.DanBuff || danDuoc.ETypeDan == ETypeDan.DanCombat)
                {
                    if (danDuoc.ETypeDan == ETypeDan.DanBuff)
                    {
                        bool isTuViType = danDuoc.EBuffType switch
                        {
                            EDanBuffType.TuVi => true,
                            EDanBuffType.TuViVaDanDocDuoiTrucCo => true,
                            EDanBuffType.TuViVaDanDocDuoiKetDan => true,
                            EDanBuffType.TuViVaDanDocDuoiNguyenAnh => true,
                            EDanBuffType.TuViVaDanDocDuoiHoaThan => true,
                            _ => false // Các loại buff không phải tu vi
                        };

                        if (isTuViType &&
                            Client.Instance.CharacterData.Stats.Ability.HasFlag(CreateCharacter.CharStats.Ability
                                .TienThienDiMach))
                        {
                            return;
                        }
                    }

                    var selectNum = Pool.Get<TabPanel>().selectNum;
                    selectNum.SetMin(0);
                    int maxValue = 0;
                    if (danDuoc.ETypeDan == ETypeDan.DanBuff || target != null)
                    {
                        maxValue = itemInventory.itemRef.Count;
                    }
                    else
                    {
                        maxValue = itemInventory.itemRef.Count <= Client.Instance.CharacterData.Progression.Level
                            ? itemInventory.itemRef.Count
                            : Client.Instance.CharacterData.Progression.Level; ///
                        if (maxValue <= 0)
                        {
                            NotifyGroup.Instance.Show(
                                $"Đã đạt tốt đa số lần sử dụng {itemInventory.iItemData.Name} hãy chiến đấu để làm mới");
                            return;
                        }
                    }

                    selectNum.SetMax(maxValue);


                    var prefix = target != null ? $"{target.Title}: " : string.Empty;
                    selectNum.ValueChanged += (value) =>
                    {
                        selectNum.SetText($"{prefix}{itemInventory.iItemData.Name} x{value}");
                    };

                    selectNum.confirmAction.RemoveAllListeners();
                    selectNum.confirmAction.AddListener(() =>
                    {
                        var value = selectNum.GetValue();
                        if (value >= 1)
                        {
                            if (target != null)
                                target.Use(danDuoc, (ushort)value);
                            else
                                Observer.Notify(EventList.Network.USE_DAN, Pool.Get<TabPanel>(),
                                    itemInventory.itemRef.Id, (ushort)value);
                            itemInventory.AddCount(-value);
                        }
                    });
                    selectNum.SetText($"{prefix}{itemInventory.iItemData.Name} x{0}");

                    selectNum.Show(0.15f);
                }
            }
        }
    }
}